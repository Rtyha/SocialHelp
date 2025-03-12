using SocialHelp.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class FamilyCard : UserControl
    {
        public FamilyCard()
        {
            InitializeComponent();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var family = (FamilyViewModel)DataContext;
            FamilyDetailsWindow detailsWindow = new FamilyDetailsWindow(family);
            detailsWindow.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var family = (FamilyViewModel)DataContext;
            FamilyEditWindow editWindow = new FamilyEditWindow(family);
            if (editWindow.ShowDialog() == true)
            {
                RefreshFamiliesList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var family = (FamilyViewModel)DataContext;
            if (MessageBox.Show($"Вы уверены, что хотите удалить семью {family.FamilyName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new SocialHelpContext())
                {
                    var familyEntity = context.Families.Find(family.Id);
                    if (familyEntity != null)
                    {
                        var childrenLinks = context.ChildrenInFamilies.Where(cif => cif.FamilyId == family.Id);
                        context.ChildrenInFamilies.RemoveRange(childrenLinks);
                        context.Families.Remove(familyEntity);
                        context.SaveChanges();
                    }
                }
                RefreshFamiliesList();
            }
        }

        private void RefreshFamiliesList()
        {
            if (this.Parent is FrameworkElement element && element.TemplatedParent is ItemsControl itemsControl)
            {
                var page = itemsControl.TemplatedParent as FamiliesPage;
                page?.LoadFamilies();
            }
        }
    }
}