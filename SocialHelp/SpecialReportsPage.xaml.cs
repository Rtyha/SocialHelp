using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SocialHelp.Models;
using Microsoft.Win32;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SocialHelp
{
    public partial class SpecialReportsPage : Page
    {
        private readonly SocialHelpContext _context;
        private readonly string reportsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SpecialReports");

        public SpecialReportsPage()
        {
            InitializeComponent();
            _context = new SocialHelpContext();
            LoadReports();
            EnsureReportsFolderExists();
        }

        private void EnsureReportsFolderExists()
        {
            if (!Directory.Exists(reportsFolder))
            {
                Directory.CreateDirectory(reportsFolder);
            }
        }

        private void LoadReports(string filter = "")
        {
            lstReports.Items.Clear();
            var files = Directory.GetFiles(reportsFolder, "*.docx")
                .Select(f => new
                {
                    FileName = Path.GetFileName(f),
                    CreationDate = File.GetCreationTime(f)
                })
                .Where(r => string.IsNullOrEmpty(filter) || r.FileName.Contains(filter))
                .ToList();

            foreach (var file in files)
            {
                var reportCard = new SpecialReportCard
                {
                    FileName = file.FileName,
                    CreationDate = file.CreationDate
                };
                lstReports.Items.Add(reportCard);
            }
        }

        private void BtnCreateReport_Click(object sender, RoutedEventArgs e)
        {
            var createWindow = new SpecialReportCreateWindow();
            if (createWindow.ShowDialog() == true)
            {
                SaveSpecialReport(createWindow.SelectedFamilyId, createWindow.SelectedChildId, createWindow.ReportDate, createWindow.ReportText);
                LoadReports(txtSearch.Text);
            }
        }

        private void SaveSpecialReport(int? familyId, int? childId, DateTime reportDate, string reportText)
        {
            using (var context = new SocialHelpContext())
            {
                var family = familyId.HasValue ? context.Families.Find(familyId) : null;
                var child = childId.HasValue ? context.Children.Find(childId) : null;

                string fileName = $"SpecialReport_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                string filePath = Path.Combine(reportsFolder, fileName);

                using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    Paragraph titlePara = body.AppendChild(new Paragraph());
                    Run titleRun = titlePara.AppendChild(new Run());
                    titleRun.AppendChild(new Text($"ОТЧЁТ ПО ПАТРОНАЖУ"));
                    titleRun.RunProperties = new RunProperties(new Bold());
                    titlePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );

                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    Paragraph familyPara = body.AppendChild(new Paragraph());
                    Run familyRun = familyPara.AppendChild(new Run());
                    familyRun.AppendChild(new Text($"Семья: {family?.FamilyName ?? "Не указана"}"));
                    familyPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    Paragraph childPara = body.AppendChild(new Paragraph());
                    Run childRun = childPara.AppendChild(new Run());
                    childRun.AppendChild(new Text($"Ребёнок: {child?.FullName ?? "Не указан"} ({child?.BirthDate?.ToString("dd.MM.yyyy") ?? "Не указана"})"));
                    childPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    Paragraph datePara = body.AppendChild(new Paragraph());
                    Run dateRun = datePara.AppendChild(new Run());
                    dateRun.AppendChild(new Text($"Дата: {reportDate.ToString("dd.MM.yyyy")}"));
                    datePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );


                    Paragraph textPara = body.AppendChild(new Paragraph());
                    Run textRun = textPara.AppendChild(new Run());
                    textRun.AppendChild(new Text($"{reportText}"));
                    textPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    Paragraph recommendationPara = body.AppendChild(new Paragraph());
                    Run recommendationRun = recommendationPara.AppendChild(new Run());
                    recommendationRun.AppendChild(new Text($"Рекомендации: Провести следующую проверку {reportDate.AddMonths(1):dd.MM.yyyy}"));
                    recommendationPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );

                    Paragraph signaturePara = body.AppendChild(new Paragraph());
                    Run signatureRun = signaturePara.AppendChild(new Run());
                    signatureRun.AppendChild(new Text($"Подготовлено: {DateTime.Now:dd.MM.yyyy}"));
                    signaturePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Right }
                    );

                    doc.Save();
                }

                MessageBox.Show($"Отчёт сохранён по пути: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadReports(txtSearch.Text);
        }
    }
}