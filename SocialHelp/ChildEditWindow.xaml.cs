using MahApps.Metro.Controls;
using SocialHelp.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SocialHelp
{
    public partial class ChildEditWindow : MetroWindow
    {
        private readonly ChildViewModel _child;
        public List<string> Genders { get; } = new List<string> { "М", "Ж" };

        public ChildEditWindow(ChildViewModel child)
        {
            InitializeComponent();
            _child = child;
            DataContext = this;

            if (_child != null)
            {
                Title = "Редактирование ребенка";
                txtFullName.Text = _child.FullName;
                dpBirthDate.SelectedDateTime = _child.BirthDate;
                cmbGender.SelectedItem = _child.Gender;
                chkHasDisability.IsChecked = _child.HasDisability;
                txtSchool.Text = _child.School;
                txtNotes.Text = _child.Notes;
            }
            else
            {
                Title = "Добавление ребенка";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new SocialHelpContext())
            {
                if (_child == null) // Добавление
                {
                    context.Children.Add(new Child
                    {
                        FullName = txtFullName.Text,
                        BirthDate = dpBirthDate.SelectedDateTime,
                        Gender = cmbGender.SelectedItem?.ToString(),
                        HasDisability = chkHasDisability.IsChecked ?? false,
                        School = txtSchool.Text,
                        Notes = txtNotes.Text
                    });
                }
                else // Редактирование
                {
                    var child = context.Children.Find(_child.Id);
                    if (child != null)
                    {
                        child.FullName = txtFullName.Text;
                        child.BirthDate = dpBirthDate.SelectedDateTime;
                        child.Gender = cmbGender.SelectedItem?.ToString();
                        child.HasDisability = chkHasDisability.IsChecked ?? false;
                        child.School = txtSchool.Text;
                        child.Notes = txtNotes.Text;
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