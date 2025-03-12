using MahApps.Metro.Controls;
using SocialHelp.Models;
using System;
using System.Linq;
using System.Windows;

namespace SocialHelp
{
    public partial class InspectionPlanEditWindow : MetroWindow
    {
        private readonly InspectionPlanViewModel _plan;
        private SocialHelpContext _context;

        public InspectionPlanEditWindow(InspectionPlanViewModel plan)
        {
            InitializeComponent();
            _plan = plan;
            _context = new SocialHelpContext();

            // Загрузка семей, сигнальных карт и комиссий
            cmbFamily.ItemsSource = _context.Families.ToList();
            cmbSignalCard.ItemsSource = _context.SignalCards.ToList();
            cmbCommission.ItemsSource = _context.Commissions.ToList();

            // Установка текущего сотрудника (авторизованного пользователя)
            if (AuthManager.CurrentUser != null)
            {
                cmbEmployee.ItemsSource = new[] { AuthManager.CurrentUser };
                cmbEmployee.SelectedItem = AuthManager.CurrentUser;
                cmbEmployee.DisplayMemberPath = "FullName"; // Убедимся, что отображается FullName
                cmbEmployee.SelectedValuePath = "EmployeeId";
            }
            else
            {
                MessageBox.Show("Текущий пользователь не авторизован! Пожалуйста, войдите в систему.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (_plan != null)
            {
                Title = "Редактирование плана";
                cmbFamily.SelectedValue = _plan.FamilyId;
                cmbSignalCard.SelectedValue = _context.InspectionPlans
                    .Where(p => p.PlanId == _plan.Id)
                    .Select(p => p.SignalCardId)
                    .FirstOrDefault();
                cmbCommission.SelectedValue = _plan.CommissionId;
                if (DateTime.TryParse(_plan.PlanDate, out var date))
                {
                    dpPlanDate.SelectedDate = date;
                }
            }
            else
            {
                Title = "Добавление плана";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamily.SelectedValue == null || cmbSignalCard.SelectedValue == null ||
                cmbEmployee.SelectedValue == null || cmbCommission.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new SocialHelpContext())
            {
                InspectionPlan planEntity;
                if (_plan == null) // Добавление
                {
                    planEntity = new InspectionPlan
                    {
                        FamilyId = (int)cmbFamily.SelectedValue,
                        SignalCardId = (int)cmbSignalCard.SelectedValue,
                        EmployeeId = AuthManager.CurrentUser.EmployeeId,
                        CommissionId = (int)cmbCommission.SelectedValue,
                        PlanDate = dpPlanDate.SelectedDate
                    };
                    context.InspectionPlans.Add(planEntity);
                }
                else // Редактирование
                {
                    planEntity = context.InspectionPlans.Find(_plan.Id);
                    if (planEntity != null)
                    {
                        planEntity.FamilyId = (int)cmbFamily.SelectedValue;
                        planEntity.SignalCardId = (int)cmbSignalCard.SelectedValue;
                        planEntity.EmployeeId = AuthManager.CurrentUser.EmployeeId;
                        planEntity.CommissionId = (int)cmbCommission.SelectedValue;
                        planEntity.PlanDate = dpPlanDate.SelectedDate;
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