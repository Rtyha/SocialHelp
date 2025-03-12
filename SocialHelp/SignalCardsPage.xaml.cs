using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using SocialHelp.Models;

namespace SocialHelp
{
    public partial class SignalCardsPage : Page
    {
        private SocialHelpContext _context;

        public SignalCardsPage()
        {
            InitializeComponent();
            _context = new SocialHelpContext();
            LoadSignalCards();
        }

        private void LoadSignalCards(string filter = "")
        {
            // Шаг 1: Загружаем данные из базы без форматирования
            var signalCardsQuery = _context.SignalCards
                .Include(sc => sc.Family)
                .Where(sc => string.IsNullOrEmpty(filter) ||
                             (sc.Family != null && sc.Family.FamilyName.Contains(filter)))
                .Select(sc => new
                {
                    Id = sc.SignalCardId,
                    FamilyName = sc.Family != null ? sc.Family.FamilyName : "Не указана",
                    SubmissionDate = sc.SubmissionDate,
                    DetectionDate = sc.DetectionDate,
                    Status = sc.Status
                })
                .ToList();

            // Шаг 2: Форматируем даты в памяти
            var signalCards = signalCardsQuery
                .Select(sc => new SignalCardViewModel
                {
                    Id = sc.Id,
                    FamilyName = sc.FamilyName,
                    SubmissionDate = sc.SubmissionDate.HasValue ? sc.SubmissionDate.Value.ToString("dd.MM.yyyy") : "Не указана",
                    DetectionDate = sc.DetectionDate.HasValue ? sc.DetectionDate.Value.ToString("dd.MM.yyyy") : "Не указана",
                    Status = sc.Status
                })
                .ToList();

            signalCardsList.ItemsSource = signalCards;
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadSignalCards(txtFilter.Text);
        }

        private void btnAddSignalCard_Click(object sender, RoutedEventArgs e)
        {
            SignalCardEditWindow editWindow = new SignalCardEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadSignalCards(txtFilter.Text);
            }
        }
    }

    public class SignalCardViewModel
    {
        public int Id { get; set; }
        public string FamilyName { get; set; }
        public string SubmissionDate { get; set; }
        public string DetectionDate { get; set; }
        public string Status { get; set; }
    }
}