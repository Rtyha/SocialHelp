using SocialHelp.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class InspectionReportsPage : Page
    {
        private SocialHelpContext _context;

        public InspectionReportsPage()
        {
            InitializeComponent();
            _context = new SocialHelpContext();
            LoadReports();
        }

        private void LoadReports(string filter = "")
        {
            var reportsQuery = _context.InspectionReports
                .Where(r => string.IsNullOrEmpty(filter) ||
                            (r.FamilyName != null && r.FamilyName.Contains(filter)))
                .Select(r => new
                {
                    r.ReportId,
                    r.InspectionDate,
                    r.FamilyName,
                    r.PlanId
                })
                .ToList(); // Выполняем запрос в память

            var reports = reportsQuery
                .Select(r => new InspectionReportViewModel
                {
                    Id = r.ReportId,
                    InspectionDate = r.InspectionDate != null ? r.InspectionDate.Value.ToString("dd.MM.yyyy") : "Не указана",
                    FamilyName = r.FamilyName ?? "Не указана",
                    PlanId = r.PlanId
                })
                .ToList();

            reportsList.ItemsSource = reports;
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadReports(txtFilter.Text);
        }

        private void btnAddReport_Click(object sender, RoutedEventArgs e)
        {
            InspectionReportEditWindow editWindow = new InspectionReportEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadReports(txtFilter.Text);
            }
        }
    }

    public class InspectionReportViewModel
    {
        public int Id { get; set; }
        public string InspectionDate { get; set; }
        public string FamilyName { get; set; }
        public int PlanId { get; set; }
    }
}