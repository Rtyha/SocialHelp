using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using SocialHelp.Models;

namespace SocialHelp
{
    public partial class InspectionPlansPage : Page
    {
        private SocialHelpContext _context;

        public InspectionPlansPage()
        {
            InitializeComponent();
            _context = new SocialHelpContext();
            LoadPlans();
        }

        private void LoadPlans(string filter = "")
        {
            var plansQuery = _context.InspectionPlans
                .Include(p => p.Family)
                .Include(p => p.SignalCard)
                .Include(p => p.Employee)
                .Include(p => p.Commission)
                .Where(p => string.IsNullOrEmpty(filter) ||
                            (p.Family != null && p.Family.FamilyName.Contains(filter)))
                .Select(p => new
                {
                    PlanId = p.PlanId,
                    FamilyName = p.Family != null ? p.Family.FamilyName : "Не указана",
                    PlanDate = p.PlanDate,
                    FamilyId = p.FamilyId,
                    CommissionId = p.CommissionId,
                    SignalCardId = p.SignalCardId,
                    EmployeeFullName = p.Employee != null ? p.Employee.FullName : "Не указан"
                })
                .ToList();

            var plans = plansQuery
                .Select(p => new InspectionPlanViewModel
                {
                    Id = p.PlanId,
                    FamilyName = p.FamilyName,
                    PlanDate = p.PlanDate.HasValue ? p.PlanDate.Value.ToString("dd.MM.yyyy") : "Не указана",
                    FamilyId = p.FamilyId,
                    CommissionId = p.CommissionId,
                    SignalCardId = p.SignalCardId,
                    EmployeeFullName = p.EmployeeFullName
                })
                .ToList();

            plansList.ItemsSource = plans;
        }
        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadPlans(txtFilter.Text);
        }

        private void btnAddPlan_Click(object sender, RoutedEventArgs e)
        {
            InspectionPlanEditWindow editWindow = new InspectionPlanEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadPlans(txtFilter.Text);
            }
        }
    }

    public class InspectionPlanViewModel
    {
        public int Id { get; set; }
        public string FamilyName { get; set; }
        public string PlanDate { get; set; }
        public int FamilyId { get; set; }
        public int CommissionId { get; set; }
        public int SignalCardId { get; set; }
        public string EmployeeFullName { get; set; }
    }
}