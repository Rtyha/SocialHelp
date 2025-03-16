using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SocialHelp.Models;

namespace SocialHelp
{
    public partial class SpecialReportCreateWindow : Window
    {
        public int? SelectedFamilyId { get; private set; }
        public int? SelectedChildId { get; private set; }
        public DateTime ReportDate { get; private set; }
        public string ReportText { get; private set; }

        public SpecialReportCreateWindow()
        {
            InitializeComponent();
            LoadFamilies();
            dpReportDate.SelectedDate = DateTime.Now;
        }

        private void LoadFamilies()
        {
            using (var context = new SocialHelpContext())
            {
                var families = context.Families.ToList();
                cbFamilies.ItemsSource = families;
                cbFamilies.DisplayMemberPath = "FamilyName";
                cbFamilies.SelectedValuePath = "FamilyId";
            }
        }

        private void CbFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFamilies.SelectedItem != null)
            {
                int familyId = (int)cbFamilies.SelectedValue;
                using (var context = new SocialHelpContext())
                {
                    var children = context.ChildrenInFamilies
                        .Where(cif => cif.FamilyId == familyId)
                        .Select(cif => cif.Child)
                        .ToList();
                    cbChildren.ItemsSource = children;
                    cbChildren.DisplayMemberPath = "FullName";
                    cbChildren.SelectedValuePath = "ChildId";
                    cbChildren.IsEnabled = true;
                }
            }
            else
            {
                cbChildren.ItemsSource = null;
                cbChildren.IsEnabled = false;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SelectedFamilyId = cbFamilies.SelectedValue as int?;
            SelectedChildId = cbChildren.SelectedValue as int?;
            ReportDate = dpReportDate.SelectedDate ?? DateTime.Now;
            ReportText = txtReportText.Text;

            if (string.IsNullOrWhiteSpace(ReportText))
            {
                MessageBox.Show("Пожалуйста, введите информацию для отчёта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}