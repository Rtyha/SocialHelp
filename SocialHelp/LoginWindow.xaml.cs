using MahApps.Metro.Controls;
using SocialHelp.Models;
using System.Linq;
using System.Windows;

namespace SocialHelp
{
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtStatus.Text = "Введите логин и пароль!";
                return;
            }

            loadingIndicator.IsActive = true;
            txtStatus.Text = "";

            using (var context = new SocialHelpContext())
            {
                var employee = context.Employees.FirstOrDefault(emp => emp.Login == login);

                if (employee == null)
                {
                    txtStatus.Text = "Неверный логин!";
                    loadingIndicator.IsActive = false;
                    return;
                }

                if (BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
                {
                    // Логирование успешного входа
                    context.ActionLogs.Add(new ActionLog
                    {
                        EmployeeId = employee.EmployeeId,
                        Action = "Вход в систему",
                        Description = $"Пользователь {employee.FullName} вошел в систему"
                    });
                    context.SaveChanges();

                    // Установка текущего пользователя
                    AuthManager.SetCurrentUser(employee);

                    // Открытие главного окна
                    MainWindow mainWindow = new MainWindow(employee);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    txtStatus.Text = "Неверный пароль!";
                }
            }
            loadingIndicator.IsActive = false;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна регистрации
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}