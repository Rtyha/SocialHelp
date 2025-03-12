using SocialHelp.Models;
using System.Windows;
using System.Windows.Controls;

namespace SocialHelp
{
    public partial class CommissionCard : UserControl
    {
        public CommissionCard()
        {
            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var commission = (CommissionViewModel)DataContext;
            CommissionEditWindow editWindow = new CommissionEditWindow(commission);
            if (editWindow.ShowDialog() == true)
            {
                RefreshCommissionsList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var commission = (CommissionViewModel)DataContext;
            if (MessageBox.Show($"Вы уверены, что хотите удалить комиссию #{commission.Id}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new SocialHelpContext())
                {
                    var commissionEntity = context.Commissions.Find(commission.Id);
                    if (commissionEntity != null)
                    {
                        context.Commissions.Remove(commissionEntity);
                        context.SaveChanges();
                    }
                }
                RefreshCommissionsList();
            }
        }

        private void RefreshCommissionsList()
        {
            if (this.Parent is FrameworkElement element && element.TemplatedParent is ItemsControl itemsControl)
            {
                var page = itemsControl.TemplatedParent as CommissionsPage;

            }
        }
    }
}