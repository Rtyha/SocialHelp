using MahApps.Metro.Controls;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using SocialHelp.Models;

namespace SocialHelp
{
    public partial class FamilyEditWindow : MetroWindow
    {
        private readonly FamilyViewModel _family;
        private SocialHelpContext _context;
        private List<Child> _allChildren; // Храним полный список детей для фильтрации

        public FamilyEditWindow(FamilyViewModel family)
        {
            InitializeComponent();
            _family = family;
            _context = new SocialHelpContext();

            // Загрузка родителей
            var fathersList = _context.Fathers.ToList();
            fathersList.Insert(0, new Father { FatherId = 0, FullName = "Не указан" });
            cmbFather.ItemsSource = fathersList;
            cmbFather.SelectedIndex = 0;

            var mothersList = _context.Mothers.ToList();
            mothersList.Insert(0, new Mother { MotherId = 0, FullName = "Не указан" });
            cmbMother.ItemsSource = mothersList;
            cmbMother.SelectedIndex = 0;

            // Загрузка детей
            _allChildren = _context.Children.ToList();
            lstChildren.ItemsSource = _allChildren;

            if (_family != null)
            {
                Title = "Редактирование семьи";
                txtFamilyName.Text = _family.FamilyName;
                cmbFather.SelectedValue = _context.Families.FirstOrDefault(f => f.FamilyId == _family.Id)?.FatherId ?? 0;
                cmbMother.SelectedValue = _context.Families.FirstOrDefault(f => f.FamilyId == _family.Id)?.MotherId ?? 0;
                cmbStatus.SelectedItem = cmbStatus.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == _family.Status);

                // Выделяем уже привязанных детей
                var linkedChildren = _context.ChildrenInFamilies
                    .Where(cif => cif.FamilyId == _family.Id)
                    .Select(cif => cif.ChildId)
                    .ToList();
                foreach (Child child in lstChildren.Items)
                {
                    if (linkedChildren.Contains(child.ChildId))
                        lstChildren.SelectedItems.Add(child);
                }
            }
            else
            {
                Title = "Добавление семьи";
            }
        }

        private void txtChildFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = txtChildFilter.Text.ToLower();
            var filteredChildren = _allChildren
                .Where(c => string.IsNullOrEmpty(filter) || c.FullName.ToLower().Contains(filter))
                .ToList();

            // Сохраняем текущие выбранные элементы
            var selectedIds = lstChildren.SelectedItems.Cast<Child>().Select(c => c.ChildId).ToList();

            // Обновляем ItemsSource с отфильтрованным списком
            lstChildren.ItemsSource = filteredChildren;

            // Восстанавливаем выбранные элементы
            foreach (Child child in lstChildren.Items)
            {
                if (selectedIds.Contains(child.ChildId))
                    lstChildren.SelectedItems.Add(child);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFamilyName.Text))
            {
                MessageBox.Show("Введите название семьи!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new SocialHelpContext())
            {
                Family familyEntity;
                if (_family == null) // Добавление
                {
                    familyEntity = new Family
                    {
                        FamilyName = txtFamilyName.Text,
                        FatherId = (int?)cmbFather.SelectedValue > 0 ? (int?)cmbFather.SelectedValue : null,
                        MotherId = (int?)cmbMother.SelectedValue > 0 ? (int?)cmbMother.SelectedValue : null,
                        Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Active"
                    };
                    context.Families.Add(familyEntity);
                    context.SaveChanges();
                }
                else // Редактирование
                {
                    familyEntity = context.Families.Find(_family.Id);
                    if (familyEntity != null)
                    {
                        familyEntity.FamilyName = txtFamilyName.Text;
                        familyEntity.FatherId = (int?)cmbFather.SelectedValue > 0 ? (int?)cmbFather.SelectedValue : null;
                        familyEntity.MotherId = (int?)cmbMother.SelectedValue > 0 ? (int?)cmbMother.SelectedValue : null;
                        familyEntity.Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Active";

                        // Удаляем старые связи с детьми
                        var oldLinks = context.ChildrenInFamilies.Where(cif => cif.FamilyId == familyEntity.FamilyId);
                        context.ChildrenInFamilies.RemoveRange(oldLinks);
                    }
                }

                // Добавляем новые связи с детьми
                foreach (Child child in lstChildren.SelectedItems)
                {
                    context.ChildrenInFamilies.Add(new ChildInFamily
                    {
                        FamilyId = familyEntity.FamilyId,
                        ChildId = child.ChildId
                    });
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