using MahApps.Metro.Controls;
using SocialHelp.Models;
using SocialHelp.Pages;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SocialHelp
{
    public partial class ParentEditWindow : MetroWindow
    {
        private readonly ParentViewModel _parent;
        public List<string> ParentTypes { get; } = new List<string> { "Мать", "Отец" };

        public ParentEditWindow(ParentViewModel parent)
        {
            InitializeComponent();
            _parent = parent;
            DataContext = this;

            if (_parent != null)
            {
                Title = "Редактирование родителя";
                cmbParentType.SelectedItem = _parent.Type;
                txtFullName.Text = _parent.FullName;
                txtAddress.Text = _parent.Address;

                using (var context = new SocialHelpContext())
                {
                    if (_parent.Type == "Мать")
                    {
                        var mother = context.Mothers.Find(_parent.Id);
                        if (mother != null)
                        {
                            dpBirthDate.SelectedDateTime = mother.BirthDate;
                            txtWorkPlace.Text = mother.WorkPlace;
                            txtPhoneNumber.Text = mother.PhoneNumber;
                            txtPassport.Text = mother.Passport;
                            txtMedicalInsurance.Text = mother.MedicalInsurance;
                            txtSNILS.Text = mother.SNILS;
                            txtINN.Text = mother.INN;
                            txtEducationLevel.Text = mother.EducationLevel;
                            txtAttitudeToChildren.Text = mother.AttitudeToChildren;
                        }
                    }
                    else
                    {
                        var father = context.Fathers.Find(_parent.Id);
                        if (father != null)
                        {
                            dpBirthDate.SelectedDateTime = father.BirthDate;
                            txtWorkPlace.Text = father.WorkPlace;
                            txtPhoneNumber.Text = father.PhoneNumber;
                            txtPassport.Text = father.Passport;
                            txtMedicalInsurance.Text = father.MedicalInsurance;
                            txtSNILS.Text = father.SNILS;
                            txtINN.Text = father.INN;
                            txtEducationLevel.Text = father.EducationLevel;
                            txtAttitudeToChildren.Text = father.AttitudeToChildren;
                        }
                    }
                }
            }
            else
            {
                Title = "Добавление родителя";
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
                if (_parent == null) // Добавление
                {
                    if (cmbParentType.SelectedItem.ToString() == "Мать")
                    {
                        context.Mothers.Add(new Mother
                        {
                            FullName = txtFullName.Text,
                            BirthDate = dpBirthDate.SelectedDateTime,
                            Address = txtAddress.Text,
                            WorkPlace = txtWorkPlace.Text,
                            PhoneNumber = txtPhoneNumber.Text,
                            Passport = txtPassport.Text,
                            MedicalInsurance = txtMedicalInsurance.Text,
                            SNILS = txtSNILS.Text,
                            INN = txtINN.Text,
                            EducationLevel = txtEducationLevel.Text,
                            AttitudeToChildren = txtAttitudeToChildren.Text
                        });
                    }
                    else
                    {
                        context.Fathers.Add(new Father
                        {
                            FullName = txtFullName.Text,
                            BirthDate = dpBirthDate.SelectedDateTime,
                            Address = txtAddress.Text,
                            WorkPlace = txtWorkPlace.Text,
                            PhoneNumber = txtPhoneNumber.Text,
                            Passport = txtPassport.Text,
                            MedicalInsurance = txtMedicalInsurance.Text,
                            SNILS = txtSNILS.Text,
                            INN = txtINN.Text,
                            EducationLevel = txtEducationLevel.Text,
                            AttitudeToChildren = txtAttitudeToChildren.Text
                        });
                    }
                }
                else // Редактирование
                {
                    if (_parent.Type == "Мать")
                    {
                        var mother = context.Mothers.Find(_parent.Id);
                        if (mother != null)
                        {
                            mother.FullName = txtFullName.Text;
                            mother.BirthDate = dpBirthDate.SelectedDateTime;
                            mother.Address = txtAddress.Text;
                            mother.WorkPlace = txtWorkPlace.Text;
                            mother.PhoneNumber = txtPhoneNumber.Text;
                            mother.Passport = txtPassport.Text;
                            mother.MedicalInsurance = txtMedicalInsurance.Text;
                            mother.SNILS = txtSNILS.Text;
                            mother.INN = txtINN.Text;
                            mother.EducationLevel = txtEducationLevel.Text;
                            mother.AttitudeToChildren = txtAttitudeToChildren.Text;
                        }
                    }
                    else
                    {
                        var father = context.Fathers.Find(_parent.Id);
                        if (father != null)
                        {
                            father.FullName = txtFullName.Text;
                            father.BirthDate = dpBirthDate.SelectedDateTime;
                            father.Address = txtAddress.Text;
                            father.WorkPlace = txtWorkPlace.Text;
                            father.PhoneNumber = txtPhoneNumber.Text;
                            father.Passport = txtPassport.Text;
                            father.MedicalInsurance = txtMedicalInsurance.Text;
                            father.SNILS = txtSNILS.Text;
                            father.INN = txtINN.Text;
                            father.EducationLevel = txtEducationLevel.Text;
                            father.AttitudeToChildren = txtAttitudeToChildren.Text;
                        }
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