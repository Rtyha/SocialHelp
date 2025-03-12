using SocialHelp.Models;
using System.Windows;
using System.Windows.Controls;

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

        private void RefreshSignalCardsList()
        {
            if (this.Parent is FrameworkElement element && element.TemplatedParent is ItemsControl itemsControl)
            {
                var page = itemsControl.TemplatedParent as SignalCardsPage;
            }
        }
    }
}