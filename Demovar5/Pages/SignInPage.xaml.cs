using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using Demovar5.Pages.Admin;
using Demovar5.Pages.Manager;
using Demovar5.Pages.Client;
using Demovar5.Pages.Guest;

namespace Demovar5.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        private MainWindow _mainWindow;

        public SignInPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _mainWindow.ChangeWindowSize(768, 1024);

            // Тестовые данные для быстрого входа (убрать в продакшене)
            LoginTextBox.Text = "94d5ous@gmail.com";
            PasswordBox.Password = "uzWC67";
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text.Trim();
            var password = PasswordBox.Password;

            // Валидация
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }

            try
            {
                using (var context = new demovar5Entities())
                {
                    var user = context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                    if (user == null)
                    {
                        Growl.Error("Неправильный логин или пароль");
                        await Task.Delay(1500);
                        Growl.Clear();
                        return;
                    }

                    Growl.Success($"Добро пожаловать, {user.FullName}!");
                    await Task.Delay(1000);
                    Growl.Clear();

                    // Переход на соответствующую страницу в зависимости от роли
                    switch (user.Roles.RoleName)
                    {
                        case "Администратор":
                            _mainWindow.MainFrame.Navigate(new AdminPage(_mainWindow, user));
                            break;
                        case "Менеджер":
                            _mainWindow.MainFrame.Navigate(new ManagerPage(_mainWindow, user));
                            break;
                        case "Авторизированный клиент":
                            _mainWindow.MainFrame.Navigate(new ClientPage(_mainWindow, user));
                            break;
                        default:
                            Growl.Warning("Неизвестная роль пользователя");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info("Вход как гость");
            _mainWindow.MainFrame.Navigate(new GuestPage(_mainWindow));
        }
    }
}
