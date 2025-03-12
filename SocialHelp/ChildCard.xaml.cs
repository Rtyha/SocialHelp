using SocialHelp.Models;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class ChildCard : UserControl
    {
        public ChildCard()
        {
            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var child = (ChildViewModel)DataContext;
            ChildEditWindow editWindow = new ChildEditWindow(child);
            if (editWindow.ShowDialog() == true)
            {
                RefreshChildrenList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var child = (ChildViewModel)DataContext;
            if (MessageBox.Show($"Вы уверены, что хотите удалить {child.FullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new SocialHelpContext())
                {
                    var childEntity = context.Children.Find(child.Id);
                    if (childEntity != null)
                    {
                        context.Children.Remove(childEntity);
                        context.SaveChanges();
                    }
                }
                RefreshChildrenList();
            }
        }

        private void RefreshChildrenList()
        {
            if (this.Parent is FrameworkElement element && element.TemplatedParent is ItemsControl itemsControl)
            {
                var page = itemsControl.TemplatedParent as ChildrenPage;
            }
        }
    }
}