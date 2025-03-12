using MahApps.Metro.Controls;
using SocialHelp.Models;
using System.Linq;
using System.Windows;

namespace SocialHelp
{
    public partial class FamilyDetailsWindow : MetroWindow
    {
        private readonly FamilyViewModel _family;

        public FamilyDetailsWindow(FamilyViewModel family)
        {
            InitializeComponent();
            _family = family;

            LoadFamilyDetails();
        }

        private void LoadFamilyDetails()
        {
            txtFamilyName.Text = _family.FamilyName;
            txtStatus.Text = _family.Status;

            using (var context = new SocialHelpContext())
            {
                var familyEntity = context.Families
                    .Include("Father")
                    .Include("Mother")
                    .FirstOrDefault(f => f.FamilyId == _family.Id);

                if (familyEntity != null)
                {
                    // Информация об отце
                    if (familyEntity.Father != null)
                    {
                        txtFatherDetails.Text = $"ФИО: {familyEntity.Father.FullName}\n" +
                                                $"Дата рождения: {familyEntity.Father.BirthDate?.ToString("dd.MM.yyyy") ?? "Не указана"}\n" +
                                                $"Адрес: {familyEntity.Father.Address ?? "Не указан"}\n" +
                                                $"Телефон: {familyEntity.Father.PhoneNumber ?? "Не указан"}";
                    }
                    else
                    {
                        txtFatherDetails.Text = "Не указан";
                    }

                    // Информация о матери
                    if (familyEntity.Mother != null)
                    {
                        txtMotherDetails.Text = $"ФИО: {familyEntity.Mother.FullName}\n" +
                                                $"Дата рождения: {familyEntity.Mother.BirthDate?.ToString("dd.MM.yyyy") ?? "Не указана"}\n" +
                                                $"Адрес: {familyEntity.Mother.Address ?? "Не указан"}\n" +
                                                $"Телефон: {familyEntity.Mother.PhoneNumber ?? "Не указан"}";
                    }
                    else
                    {
                        txtMotherDetails.Text = "Не указан";
                    }

                    // Список детей
                    var children = context.ChildrenInFamilies
                        .Where(cif => cif.FamilyId == _family.Id)
                        .Select(cif => cif.Child)
                        .ToList();
                    childrenList.ItemsSource = children;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}