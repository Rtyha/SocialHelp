using System;
using System.Windows;
using System.Windows.Controls;
using SocialHelp.Models;
using Microsoft.Win32;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace SocialHelp
{
    public partial class SignalCardCard : UserControl
    {
        public SignalCardCard()
        {
            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var signalCard = (SignalCardViewModel)DataContext;
            SignalCardEditWindow editWindow = new SignalCardEditWindow(signalCard);
            if (editWindow.ShowDialog() == true)
            {
                RefreshSignalCardsList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var signalCard = (SignalCardViewModel)DataContext;
            if (MessageBox.Show($"Вы уверены, что хотите удалить сигнальную карту #{signalCard.Id}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new SocialHelpContext())
                {
                    var signalCardEntity = context.SignalCards.Find(signalCard.Id);
                    if (signalCardEntity != null)
                    {
                        context.SignalCards.Remove(signalCardEntity);
                        context.SaveChanges();
                    }
                }
                RefreshSignalCardsList();
            }
        }

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            var signalCard = (SignalCardViewModel)DataContext;
            if (signalCard == null)
            {
                MessageBox.Show("Не удалось получить данные сигнальной карты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Шаг 1: Получаем полные данные сигнальной карты из базы
            using (var context = new SocialHelpContext())
            {
                var signalCardEntity = context.SignalCards
                    .Include("Family") // Используем строку вместо лямбда-выражения
                    .FirstOrDefault(sc => sc.SignalCardId == signalCard.Id);

                if (signalCardEntity == null)
                {
                    MessageBox.Show("Сигнальная карта не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Шаг 2: Открываем диалог сохранения файла
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Word Document (*.docx)|*.docx",
                    FileName = $"SignalCardReport_{signalCard.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.docx",
                    DefaultExt = "docx",
                    Title = "Выберите место для сохранения отчёта"
                };

                if (saveFileDialog.ShowDialog() != true)
                {
                    return; // Пользователь отменил выбор
                }

                string filePath = saveFileDialog.FileName;

                // Шаг 3: Создаём отчёт в формате .docx
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
                        titleRun.AppendChild(new Text("Сигнальная карта семьи, находящейся в социально опасном положении, для постановки на межведомственный учет"));
                        titleRun.RunProperties = new RunProperties(new Bold());
                        titlePara.ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center }
                        );

                        // Пустая строка
                        body.AppendChild(new Paragraph(new Run(new Text(""))));

                        // Дата направления
                        Paragraph submissionDatePara = body.AppendChild(new Paragraph());
                        Run submissionDateRun = submissionDatePara.AppendChild(new Run());
                        submissionDateRun.AppendChild(new Text($"Дата направления: {signalCardEntity.SubmissionDate?.ToString("dd.MM.yyyy") ?? "Не указана"}"));
                        submissionDatePara.ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        );

                        // Дата выявления
                        Paragraph detectionDatePara = body.AppendChild(new Paragraph());
                        Run detectionDateRun = detectionDatePara.AppendChild(new Run());
                        detectionDateRun.AppendChild(new Text($"Дата выявления: {signalCardEntity.DetectionDate?.ToString("dd.MM.yyyy") ?? "Не указана"}"));
                        detectionDatePara.ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        );

                        // Фамилия семьи
                        Paragraph familyNamePara = body.AppendChild(new Paragraph());
                        Run familyNameRun = familyNamePara.AppendChild(new Run());
                        familyNameRun.AppendChild(new Text($"Фамилия семьи, находящейся в СОП: {signalCardEntity.Family?.FamilyName ?? "Не указана"}"));
                        familyNamePara.ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        );

                        // Адрес проживания
                        Paragraph addressPara = body.AppendChild(new Paragraph());
                        Run addressRun = addressPara.AppendChild(new Run());
                        addressRun.AppendChild(new Text($"Адрес проживания: {signalCardEntity.Address ?? "Не указан"}"));
                        addressPara.ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        );

                        // ФИО сотрудника, выявившего семью
                        Paragraph detectedByPara = body.AppendChild(new Paragraph());
                        Run detectedByRun = detectedByPara.AppendChild(new Run());
                        detectedByRun.AppendChild(new Text($"ФИО сотрудника, выявившего семью: {signalCardEntity.DetectedByEmployee ?? "Не указан"}"));
                        detectedByPara.ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        );

                        // Дата рассмотрения
                        Paragraph reviewDatePara = body.AppendChild(new Paragraph());
                        Run reviewDateRun = reviewDatePara.AppendChild(new Run());
                        reviewDateRun.AppendChild(new Text($"Дата рассмотрения сигнальной карты: {signalCardEntity.ReviewDate?.ToString("dd.MM.yyyy") ?? "Не указана"}"));
                        reviewDatePara.ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        );

                        // ФИО сотрудника, рассмотревшего карту
                        Paragraph reviewedByPara = body.AppendChild(new Paragraph());
                        Run reviewedByRun = reviewedByPara.AppendChild(new Run());
                        reviewedByRun.AppendChild(new Text($"ФИО сотрудника, рассмотревшего сигнальную карту: {signalCardEntity.ReviewedByEmployee ?? "Не указан"}"));
                        reviewedByPara.ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        );

                        doc.Save();
                    }

                    MessageBox.Show($"Отчёт успешно сохранён по пути: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshSignalCardsList()
        {
            if (this.Parent is FrameworkElement element && element.TemplatedParent is ItemsControl itemsControl)
            {
                var page = itemsControl.TemplatedParent as SignalCardsPage;
                if (page != null)
                {
                }
            }
        }
    }
}