using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SocialHelp.Models;

namespace SocialHelp
{
    public partial class SpecialReportCard : UserControl
    {
        public SpecialReportCard()
        {
            InitializeComponent();
            // Устанавливаем DataContext на сам UserControl, чтобы привязка работала
            DataContext = this;
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(SpecialReportCard), new PropertyMetadata(""));

        public DateTime CreationDate
        {
            get { return (DateTime)GetValue(CreationDateProperty); }
            set { SetValue(CreationDateProperty, value); }
        }

        public static readonly DependencyProperty CreationDateProperty =
            DependencyProperty.Register("CreationDate", typeof(DateTime), typeof(SpecialReportCard), new PropertyMetadata(DateTime.Now));

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SpecialReports", FileName);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Файл не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}