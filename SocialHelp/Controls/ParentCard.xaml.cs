using SocialHelp.Models;
using SocialHelp.Pages;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class ParentCard : UserControl
    {
        public ParentCard()
        {
            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = (ParentViewModel)DataContext;
            ParentEditWindow editWindow = new ParentEditWindow(parent);
            if (editWindow.ShowDialog() == true)
            {
                RefreshParentList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = (ParentViewModel)DataContext;
            if (MessageBox.Show($"Вы уверены, что хотите удалить {parent.FullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new SocialHelpContext())
                {
                    if (parent.Type == "Мать")
                    {
                        var mother = context.Mothers.Find(parent.Id);
                        if (mother != null) context.Mothers.Remove(mother);
                    }
                    else
                    {
                        var father = context.Fathers.Find(parent.Id);
                        if (father != null) context.Fathers.Remove(father);
                    }
                    context.SaveChanges();
                }
                RefreshParentList();
            }
        }

        private void RefreshParentList()
        {
            if (this.Parent is FrameworkElement element && element.TemplatedParent is ItemsControl itemsControl)
            {
                var page = itemsControl.TemplatedParent as ParentsPage;

            }
        }
    }
}