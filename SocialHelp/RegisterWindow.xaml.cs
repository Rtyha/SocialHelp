using MahApps.Metro.Controls;
using SocialHelp.Models;
using System.Linq;
using System.Windows;

namespace SocialHelp
{
    public partial class RegisterWindow : MetroWindow
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string login = txtLogin.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtStatus.Text = "Заполните все поля!";
                return;
            }

            loadingIndicator.IsActive = true;
            txtStatus.Text = "";

            using (var context = new SocialHelpContext())
            {
                // Проверка уникальности логина
                if (context.Employees.Any(emp => emp.Login == login))
                {
                    txtStatus.Text = "Этот логин уже занят!";
                    loadingIndicator.IsActive = false;
                    return;
                }

                // Создание нового сотрудника
                var newEmployee = new Employee
                {
                    FullName = fullName,
                    Login = login,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Position = "Новый сотрудник", // Можно сделать выбор должности
                    Role = "Employee" // По умолчанию обычный сотрудник
                };

                context.Employees.Add(newEmployee);
                context.SaveChanges();

                // Логирование регистрации
                context.ActionLogs.Add(new ActionLog
                {
                    EmployeeId = newEmployee.EmployeeId,
                    Action = "Регистрация",
                    Description = $"Зарегистрирован новый пользователь: {newEmployee.FullName}"
                });
                context.SaveChanges();

                txtStatus.Foreground = System.Windows.Media.Brushes.Green;
                txtStatus.Text = "Регистрация успешна! Теперь вы можете войти.";
                loadingIndicator.IsActive = false;

                // Очистка полей
                txtFullName.Text = "";
                txtLogin.Text = "";
                txtPassword.Password = "";
            }
        }
    }
}