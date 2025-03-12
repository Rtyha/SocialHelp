using System;
using System.Windows;
using System.Windows.Controls;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using Microsoft.Win32;
using System.Data.Entity;
using SocialHelp.Models;
using System.Linq;
using iText.IO.Font;

namespace SocialHelp
{
    public partial class InspectionPlanCard : System.Windows.Controls.UserControl
    {
        public InspectionPlanCard()
        {
            InitializeComponent();
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var planViewModel = (InspectionPlanViewModel)DataContext;
            if (planViewModel == null) return;

            using (var context = new SocialHelpContext())
            {
                var planEntity = context.InspectionPlans
                    .Include("Family")
                    .Include("SignalCard")
                    .Include("Employee")
                    .Include("Commission")
                    .FirstOrDefault(p => p.PlanId == planViewModel.Id);
                if (planEntity == null)
                {
                    MessageBox.Show("План не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                GeneratePdfReport(planEntity);
            }
        }

        private void GeneratePdfReport(InspectionPlan plan)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"InspectionPlan_{plan.PlanId}.pdf",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveFileDialog.ShowDialog() != true) return;
                string filePath = saveFileDialog.FileName;

                // Используем Arial с поддержкой кириллицы
                PdfFont font = PdfFontFactory.CreateFont("C:/Windows/Fonts/arial.ttf", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                PdfFont boldFont = PdfFontFactory.CreateFont("C:/Windows/Fonts/arialbd.ttf", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

                using (var writer = new PdfWriter(filePath))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf))
                {
                    // Заголовок документа
                    document.Add(new Paragraph("ПЛАН ОБСЛЕДОВАНИЯ СЕМЬИ")
                        .SetFont(boldFont)
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetMarginBottom(20));

                    // Блок с общими данными
                    document.Add(new Paragraph("Общие данные")
                        .SetFont(boldFont)
                        .SetFontSize(14)
                        .SetMarginBottom(10));

                    // Используем Paragraph вместо Table для упрощения
                    document.Add(new Paragraph($"Номер плана: {plan.PlanId}")
                        .SetFont(font)
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Дата плана: {plan.PlanDate?.ToString("dd.MM.yyyy") ?? "Не указана"}")
                        .SetFont(font)
                        .SetFontSize(12));

                    // Блок с деталями
                    document.Add(new Paragraph("Детали")
                        .SetFont(boldFont)
                        .SetFontSize(14)
                        .SetMarginTop(20)
                        .SetMarginBottom(10));

                    // Отладка данных
                    MessageBox.Show($"Метка: Семья, Значение: {plan.Family?.FamilyName ?? "Не указана"}");
                    MessageBox.Show($"Метка: Сигнальная карта, Значение: #{plan.SignalCard?.SignalCardId ?? 0}");

                    // Добавляем данные с метками
                    document.Add(new Paragraph($"Семья: {plan.Family?.FamilyName ?? "Не указана"}")
                        .SetFont(font)
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Сигнальная карта: #{plan.SignalCard?.SignalCardId ?? 0}")
                        .SetFont(font)
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Сотрудник: {plan.Employee?.FullName ?? "Не указан"}")
                        .SetFont(font)
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Сотрудник опеки: {plan.Commission?.GuardianshipEmployee ?? "Не указан"}")
                        .SetFont(font)
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Сотрудник ПДН: {plan.Commission?.PDNEmployee ?? "Не указан"}")
                        .SetFont(font)
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Сотрудник КДН: {plan.Commission?.KDNEmployee ?? "Не указан"}")
                        .SetFont(font)
                        .SetFontSize(12));
                }

                MessageBox.Show($"Отчёт сохранён по пути: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта: {ex.ToString()}\nСтек: {ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var plan = (InspectionPlanViewModel)DataContext;
            InspectionPlanEditWindow editWindow = new InspectionPlanEditWindow(plan);
            if (editWindow.ShowDialog() == true)
            {
                RefreshPlansList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var plan = (InspectionPlanViewModel)DataContext;
            if (MessageBox.Show($"Вы уверены, что хотите удалить план #{plan.Id}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new SocialHelpContext())
                {
                    var planEntity = context.InspectionPlans.Find(plan.Id);
                    if (planEntity != null)
                    {
                        context.InspectionPlans.Remove(planEntity);
                        context.SaveChanges();
                    }
                }
                RefreshPlansList();
            }
        }

        private void RefreshPlansList()
        {
            if (this.Parent is FrameworkElement element && element.TemplatedParent is ItemsControl itemsControl)
            {
                var page = itemsControl.TemplatedParent as InspectionPlansPage;

            }
        }
    }
}