using MahApps.Metro.Controls;
using SocialHelp.Models;
using System;
using System.Linq;
using System.Windows;

namespace SocialHelp
{
    public partial class InspectionReportEditWindow : MetroWindow
    {
        private readonly InspectionReportViewModel _report;
        private SocialHelpContext _context;

        public InspectionReportEditWindow(InspectionReportViewModel report)
        {
            InitializeComponent();
            _report = report;
            _context = new SocialHelpContext();

            // Загрузка планов обследования
            cmbPlan.ItemsSource = _context.InspectionPlans.ToList();

            if (_report != null)
            {
                Title = "Редактирование акта";
                cmbPlan.SelectedValue = _report.PlanId;
                if (DateTime.TryParse(_report.InspectionDate, out var date))
                {
                    dpInspectionDate.SelectedDate = date;
                }
                else
                {
                    dpInspectionDate.SelectedDate = null;
                }

                var reportEntity = _context.InspectionReports.Find(_report.Id);
                if (reportEntity != null)
                {
                    txtCitizenComplaints.Text = reportEntity.CitizenComplaints;
                    txtPDNMaterials.Text = reportEntity.PDNMaterials;
                    txtKDNMaterials.Text = reportEntity.KDNMaterials;
                    txtFamilyName.Text = reportEntity.FamilyName;
                }
            }
            else
            {
                Title = "Добавление акта";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPlan.SelectedValue == null)
            {
                MessageBox.Show("Выберите план обследования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new SocialHelpContext())
            {
                InspectionReport reportEntity;
                if (_report == null) // Добавление
                {
                    reportEntity = new InspectionReport
                    {
                        PlanId = (int)cmbPlan.SelectedValue,
                        InspectionDate = dpInspectionDate.SelectedDate,
                        CitizenComplaints = string.IsNullOrWhiteSpace(txtCitizenComplaints.Text) ? null : txtCitizenComplaints.Text,
                        PDNMaterials = string.IsNullOrWhiteSpace(txtPDNMaterials.Text) ? null : txtPDNMaterials.Text,
                        KDNMaterials = string.IsNullOrWhiteSpace(txtKDNMaterials.Text) ? null : txtKDNMaterials.Text,
                        FamilyName = string.IsNullOrWhiteSpace(txtFamilyName.Text) ? null : txtFamilyName.Text
                    };
                    context.InspectionReports.Add(reportEntity);
                }
                else // Редактирование
                {
                    reportEntity = context.InspectionReports.Find(_report.Id);
                    if (reportEntity != null)
                    {
                        reportEntity.PlanId = (int)cmbPlan.SelectedValue;
                        reportEntity.InspectionDate = dpInspectionDate.SelectedDate;
                        reportEntity.CitizenComplaints = string.IsNullOrWhiteSpace(txtCitizenComplaints.Text) ? null : txtCitizenComplaints.Text;
                        reportEntity.PDNMaterials = string.IsNullOrWhiteSpace(txtPDNMaterials.Text) ? null : txtPDNMaterials.Text;
                        reportEntity.KDNMaterials = string.IsNullOrWhiteSpace(txtKDNMaterials.Text) ? null : txtKDNMaterials.Text;
                        reportEntity.FamilyName = string.IsNullOrWhiteSpace(txtFamilyName.Text) ? null : txtFamilyName.Text;
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