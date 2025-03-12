using SocialHelp.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class CommissionsPage : Page
    {
        private SocialHelpContext _context;

        public CommissionsPage()
        {
            InitializeComponent();
            _context = new SocialHelpContext();
            LoadCommissions();
        }

        private void LoadCommissions(string filter = "")
        {
            var commissions = _context.Commissions
                .Where(c => string.IsNullOrEmpty(filter) ||
                            (c.GuardianshipEmployee != null && c.GuardianshipEmployee.Contains(filter)) ||
                            (c.PDNEmployee != null && c.PDNEmployee.Contains(filter)) ||
                            (c.KDNEmployee != null && c.KDNEmployee.Contains(filter)))
                .Select(c => new CommissionViewModel
                {
                    Id = c.CommissionId,
                    GuardianshipEmployee = c.GuardianshipEmployee ?? "Не указан",
                    PDNEmployee = c.PDNEmployee ?? "Не указан",
                    KDNEmployee = c.KDNEmployee ?? "Не указан"
                })
                .ToList();

            commissionsList.ItemsSource = commissions;
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCommissions(txtFilter.Text);
        }

        private void btnAddCommission_Click(object sender, RoutedEventArgs e)
        {
            CommissionEditWindow editWindow = new CommissionEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadCommissions(txtFilter.Text);
            }
        }
    }

    public class CommissionViewModel
    {
        public int Id { get; set; }
        public string GuardianshipEmployee { get; set; }
        public string PDNEmployee { get; set; }
        public string KDNEmployee { get; set; }
    }
}