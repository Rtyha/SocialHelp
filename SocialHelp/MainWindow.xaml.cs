using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks; // Добавляем для iconPacks
using SocialHelp.Models;
using SocialHelp.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SocialHelp
{
    public partial class MainWindow : MetroWindow
    {
        private readonly Employee _currentEmployee;

        public MainWindow(Employee employee)
        {
            InitializeComponent();
            _currentEmployee = employee;

            // Устанавливаем начальную страницу и заголовок
            txtPageTitle.Text = "Семьи";
            txtUserName.Text = _currentEmployee.FullName; // Устанавливаем ФИО
            PageContent.Navigate(new FamiliesPage());
        }

        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListView.SelectedItem == null) return;

            ListViewItem selectedItem = MenuListView.SelectedItem as ListViewItem;

            // Обновляем страницу и заголовок в зависимости от выбранного пункта меню
            if (selectedItem == menuFamilies)
            {
                PageContent.Navigate(new FamiliesPage());
                txtPageTitle.Text = "Семьи";
            }
            else if (selectedItem == menuChildren)
            {
                PageContent.Navigate(new ChildrenPage());
                txtPageTitle.Text = "Дети";
            }
            else if (selectedItem == menuParents)
            {
                PageContent.Navigate(new ParentsPage());
                txtPageTitle.Text = "Родители";
            }
            else if (selectedItem == menuSignalCards)
            {
                PageContent.Navigate(new SignalCardsPage());
                txtPageTitle.Text = "Сигнальные карты";
            }
            else if (selectedItem == menuInspectionPlans)
            {
                PageContent.Navigate(new InspectionPlansPage());
                txtPageTitle.Text = "Планы обследования";
            }
            else if (selectedItem == menuInspectionReports)
            {
                PageContent.Navigate(new InspectionReportsPage());
                txtPageTitle.Text = "Акты обследования";
            }
            else if (selectedItem == menuCommissions)
            {
                PageContent.Navigate(new CommissionsPage());
                txtPageTitle.Text = "Комиссии";
            }

            // Сбрасываем выбор, чтобы можно было снова выбрать тот же пункт
            MenuListView.SelectedItem = null;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Предполагаем, что LoginWindow находится в пространстве имён SocialHelp
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}