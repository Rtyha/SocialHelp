using System.Windows;
using System.Windows.Controls;
using SocialHelp.Models;
using System.Data.Entity;
using System.Linq;

namespace SocialHelp
{
    public partial class InspectionReportCard : UserControl
    {
        public InspectionReportCard()
        {
            InitializeComponent();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var report = (InspectionReportViewModel)DataContext;

            using (var context = new SocialHelpContext())
            {
                var reportEntity = context.InspectionReports
                    .Include(r => r.InspectionPlan)
                    .Include(r => r.InspectionPlan.Family)
                    .Include(r => r.InspectionPlan.Family.ChildrenInFamilies)
                    .Include(r => r.InspectionPlan.Family.ChildrenInFamilies.Select(cif => cif.Child))
                    .Include(r => r.InspectionPlan.Employee)
                    .Include(r => r.InspectionPlan.SignalCard)
                    .FirstOrDefault(r => r.ReportId == report.Id);
                if (reportEntity == null)
                {
                    MessageBox.Show("Отчёт не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Формируем данные для отображения в окне
                string familyName = reportEntity.InspectionPlan?.Family?.FamilyName ?? "Не указана";
                string employeeName = reportEntity.InspectionPlan?.Employee != null
                    ? $"{reportEntity.InspectionPlan.Employee.FullName} (19.03.1992)"
                    : "Не указан";
                string childName = reportEntity.InspectionPlan?.Family?.ChildrenInFamilies?.FirstOrDefault()?.Child != null
                    ? $"{reportEntity.InspectionPlan.Family.ChildrenInFamilies.FirstOrDefault().Child.FullName} ({reportEntity.InspectionPlan.Family.ChildrenInFamilies.FirstOrDefault().Child.BirthDate?.ToString("dd.MM.yyyy") ?? "06.04.2008"})"
                    : "Не указан";

                // Открываем окно с деталями
                var detailsWindow = new InspectionReportDetailsWindow(reportEntity, familyName, employeeName, childName);
                detailsWindow.ShowDialog();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var report = (InspectionReportViewModel)DataContext;
            InspectionReportEditWindow editWindow = new InspectionReportEditWindow(report);
            if (editWindow.ShowDialog() == true)
            {
                RefreshReportsList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var report = (InspectionReportViewModel)DataContext;
            if (MessageBox.Show($"Вы уверены, что хотите удалить акт #{report.Id}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new SocialHelpContext())
                {
                    var reportEntity = context.InspectionReports.Find(report.Id);
                    if (reportEntity != null)
                    {
                        context.InspectionReports.Remove(reportEntity);
                        context.SaveChanges();
                    }
                }
                RefreshReportsList();
            }
        }

        private void RefreshReportsList()
        {
            if (this.Parent is FrameworkElement element && element.TemplatedParent is ItemsControl itemsControl)
            {
                var page = itemsControl.TemplatedParent as InspectionReportsPage;            }
        }
    }
}