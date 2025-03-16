using SocialHelp.Models;
using System;
using System.Windows;

namespace SocialHelp
{
    public partial class InspectionReportInputWindow : Window
    {
        public string MainText { get; private set; }
        public InspectionReport Report { get; private set; }

        public InspectionReportInputWindow(InspectionReport report, string familyName, string employeeName, string childName, string registrationDate)
        {
            InitializeComponent();
            Report = report;

            // Заполняем поля данными
            txtFamilyName.Text = familyName;
            txtEmployeeName.Text = employeeName;
            txtChildName.Text = childName;
            txtRegistrationDate.Text = registrationDate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MainText = txtMainText.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}