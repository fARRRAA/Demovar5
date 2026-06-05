using System.Windows;
using System.Windows.Controls;
using Demovar5.Pages.Guest;

namespace Demovar5.Pages.Client
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private MainWindow _mainWindow;
        private Users _currentUser;

        public string UserFullName => _currentUser?.FullName ?? "Клиент";

        public ClientPage(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = user;
            _mainWindow.ChangeWindowSize(768, 1024);

            DataContext = this;

            // Загружаем страницу просмотра товаров
            ContentFrame.Navigate(new GuestPage(_mainWindow));
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", 
                "Подтверждение", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _mainWindow.MainFrame.Navigate(new SignInPage(_mainWindow));
            }
        }
    }
}
