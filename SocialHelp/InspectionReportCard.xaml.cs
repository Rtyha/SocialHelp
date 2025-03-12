using System;
using System.Windows;
using System.Windows.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SocialHelp.Models;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
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

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var report = (InspectionReportViewModel)DataContext;

            using (var context = new SocialHelpContext())
            {
                var reportEntity = context.InspectionReports
                    .Include(r => r.InspectionPlan)
                    .FirstOrDefault(r => r.ReportId == report.Id);
                if (reportEntity == null) return;

                GenerateWordReport(reportEntity);
            }
        }

        private void GenerateWordReport(InspectionReport report)
        {
            try
            {
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"InspectionReport_{report.ReportId}.docx");

                using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Заголовок
                    Paragraph titlePara = body.AppendChild(new Paragraph());
                    Run titleRun = titlePara.AppendChild(new Run());
                    titleRun.AppendChild(new Text("АКТ ОБСЛЕДОВАНИЯ СЕМЬИ"));
                    titleRun.RunProperties = new RunProperties(new Bold());
                    titlePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );

                    // Номер
                    Paragraph idPara = body.AppendChild(new Paragraph());
                    Run idRun = idPara.AppendChild(new Run());
                    idRun.AppendChild(new Text($"№ {report.ReportId}"));
                    idPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    // Дата
                    Paragraph datePara = body.AppendChild(new Paragraph());
                    Run dateRun = datePara.AppendChild(new Run());
                    dateRun.AppendChild(new Text(report.InspectionDate?.ToString("dd.MM.yyyy") ?? DateTime.Now.ToString("dd.MM.yyyy")));
                    datePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Right }
                    );

                    // Пустая строка
                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    // Жалобы и материалы
                    body.AppendChild(new Paragraph(new Run(new Text($"1. Жалобы и заявления граждан {report.CitizenComplaints ?? "_________________________"}"))));
                    body.AppendChild(new Paragraph(new Run(new Text($"2. Материалы ПДН {report.PDNMaterials ?? "_________________________"}"))));
                    body.AppendChild(new Paragraph(new Run(new Text($"3. Материалы КДН {report.KDNMaterials ?? "_________________________"}"))));

                    // Пустая строка
                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    // Фамилия семьи
                    Paragraph familyPara = body.AppendChild(new Paragraph());
                    Run familyRun = familyPara.AppendChild(new Run());
                    familyRun.AppendChild(new Text($"({report.InspectionPlan?.Family?.FamilyName ?? "фамилия семьи"})"));
                    familyPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
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
                var page = itemsControl.TemplatedParent as InspectionReportsPage;

            }
        }

    }
}