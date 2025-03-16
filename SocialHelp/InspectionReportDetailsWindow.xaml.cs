using System;
using System.Linq;
using System.Windows;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32; // Для SaveFileDialog
using SocialHelp.Models;

namespace SocialHelp
{
    public partial class InspectionReportDetailsWindow : Window
    {
        private readonly InspectionReport _report;

        public InspectionReportDetailsWindow(InspectionReport report, string familyName, string employeeName, string childName)
        {
            InitializeComponent();
            _report = report;

            // Заполняем поля данными
            txtReportId.Text = report.ReportId.ToString();
            txtFamilyName.Text = familyName;
            txtInspectionDate.Text = report.InspectionDate?.ToString("dd.MM.yyyy") ?? "Не указана";
            txtEmployeeName.Text = employeeName;
            txtChildName.Text = childName;
            txtCitizenComplaints.Text = report.CitizenComplaints ?? "Не указано";
            txtPDNMaterials.Text = report.PDNMaterials ?? "Не указано";
            txtKDNMaterials.Text = report.KDNMaterials ?? "Не указано";
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно для ввода основного текста отчёта
            string familyName = txtFamilyName.Text;
            string employeeName = txtEmployeeName.Text;
            string childName = txtChildName.Text;
            string registrationDate = _report.InspectionPlan?.SignalCard?.SubmissionDate?.ToString("yyyy") ?? "2019";

            var inputWindow = new InspectionReportInputWindow(_report, familyName, employeeName, childName, registrationDate);
            if (inputWindow.ShowDialog() == true)
            {
                // Открываем диалоговое окно для выбора пути сохранения
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = $"InspectionReport_{_report.ReportId}.docx",
                    Filter = "Word Documents (*.docx)|*.docx",
                    Title = "Сохранить отчёт",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    GenerateWordReport(_report, inputWindow.MainText, filePath);
                }
            }
        }

        private void GenerateWordReport(InspectionReport report, string mainText, string filePath)
        {
            try
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Заголовок
                    Paragraph titlePara = body.AppendChild(new Paragraph());
                    Run titleRun = titlePara.AppendChild(new Run());
                    titleRun.AppendChild(new Text($"ОТЧЁТ ПО ВЫПОЛНЕНИЮ ПРОФИЛАКТИЧЕСКОЙ РАБОТЫ С СЕМЬЁЙ {report.InspectionPlan?.Family?.FamilyName?.ToUpper() ?? "ТУРЫБКОВ"}"));
                    titleRun.RunProperties = new RunProperties(new Bold());
                    titlePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );

                    // Пустая строка
                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    // ФИО сотрудника (убираем дату рождения)
                    Paragraph employeePara = body.AppendChild(new Paragraph());
                    Run employeeRun = employeePara.AppendChild(new Run());
                    employeeRun.AppendChild(new Text($"{report.InspectionPlan?.Employee?.FullName ?? "Анастас Владимировной"}"));
                    employeePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    // ФИО ребёнка с использованием поля School
                    Paragraph childPara = body.AppendChild(new Paragraph());
                    Run childRun = childPara.AppendChild(new Run());
                    var child = report.InspectionPlan?.Family?.ChildrenInFamilies?.FirstOrDefault()?.Child;
                    childRun.AppendChild(new Text($"{child?.FullName ?? "Натальей Сергеевной"}, учащейся {child?.School ?? "МАОУ СОШ г. п. Софрадно"} ({child?.BirthDate?.ToString("dd.MM.yyyy") ?? "06.04.2008"}) г.р."));
                    childPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    // Пустая строка
                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    // Дата постановки на учёт
                    Paragraph regDatePara = body.AppendChild(new Paragraph());
                    Run regDateRun = regDatePara.AppendChild(new Run());
                    regDateRun.AppendChild(new Text($"стоящей на учете в КДН как семья СОП за {report.InspectionPlan?.SignalCard?.SubmissionDate?.ToString("yyyy") ?? "2019"} года"));
                    regDatePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    // Пустая строка
                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    // Основной текст
                    Paragraph mainTextPara = body.AppendChild(new Paragraph());
                    Run mainTextRun = mainTextPara.AppendChild(new Run());
                    mainTextRun.AppendChild(new Text(mainText));
                    mainTextPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    // Пустая строка
                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    // Подпись
                    Paragraph signaturePara = body.AppendChild(new Paragraph());
                    Run signatureRun = signaturePara.AppendChild(new Run());
                    signatureRun.AppendChild(new Text($"{report.InspectionDate?.ToString("dd.MM.yyyy") ?? "20.09.2019"} г."));
                    signaturePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Right }
                    );

                    doc.Save();
                }

                MessageBox.Show($"Отчёт сохранён по пути: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта: {ex.ToString()}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}