using MahApps.Metro.Controls;
using SocialHelp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class SignalCardEditWindow : MetroWindow
    {
        private readonly SignalCardViewModel _signalCard;
        private SocialHelpContext _context;

        public SignalCardEditWindow(SignalCardViewModel signalCard)
        {
            InitializeComponent();
            _signalCard = signalCard;
            _context = new SocialHelpContext();

            // Загрузка семей
            cmbFamily.ItemsSource = _context.Families.ToList();

            if (_signalCard != null)
            {
                Title = "Редактирование сигнальной карты";
                cmbFamily.SelectedValue = _context.SignalCards
                    .Where(sc => sc.SignalCardId == _signalCard.Id)
                    .Select(sc => sc.FamilyId)
                    .FirstOrDefault();
                dpSubmissionDate.SelectedDate = DateTime.TryParse(_signalCard.SubmissionDate, out var submissionDate) ? submissionDate : (DateTime?)null;
                dpDetectionDate.SelectedDate = DateTime.TryParse(_signalCard.DetectionDate, out var detectionDate) ? detectionDate : (DateTime?)null;
                cmbStatus.SelectedItem = cmbStatus.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == _signalCard.Status);
            }
            else
            {
                Title = "Добавление сигнальной карты";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamily.SelectedValue == null || cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите семью и статус!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new SocialHelpContext())
            {
                SignalCard signalCardEntity;
                if (_signalCard == null) // Добавление
                {
                    signalCardEntity = new SignalCard
                    {
                        FamilyId = (int)cmbFamily.SelectedValue,
                        SubmissionDate = dpSubmissionDate.SelectedDate,
                        DetectionDate = dpDetectionDate.SelectedDate,
                        Address = txtAddress.Text,
                        DetectedByEmployee = txtDetectedByEmployee.Text,
                        ReviewDate = dpReviewDate.SelectedDate,
                        ReviewedByEmployee = txtReviewedByEmployee.Text,
                        Description = txtDescription.Text,
                        Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "InProgress"
                    };
                    context.SignalCards.Add(signalCardEntity);
                }
                else // Редактирование
                {
                    signalCardEntity = context.SignalCards.Find(_signalCard.Id);
                    if (signalCardEntity != null)
                    {
                        signalCardEntity.FamilyId = (int)cmbFamily.SelectedValue;
                        signalCardEntity.SubmissionDate = dpSubmissionDate.SelectedDate;
                        signalCardEntity.DetectionDate = dpDetectionDate.SelectedDate;
                        signalCardEntity.Address = txtAddress.Text;
                        signalCardEntity.DetectedByEmployee = txtDetectedByEmployee.Text;
                        signalCardEntity.ReviewDate = dpReviewDate.SelectedDate;
                        signalCardEntity.ReviewedByEmployee = txtReviewedByEmployee.Text;
                        signalCardEntity.Description = txtDescription.Text;
                        signalCardEntity.Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? signalCardEntity.Status;
                    }
                }
                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}