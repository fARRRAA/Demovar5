using System.Windows;
using System.Windows.Controls;

namespace Demovar5.Pages.Manager
{
    /// <summary>
    /// Логика взаимодействия для ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {
        private MainWindow _mainWindow;
        private Users _currentUser;

        public string UserFullName => _currentUser?.FullName ?? "Менеджер";

        public ManagerPage(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = user;
            _mainWindow.ChangeWindowSize(900, 1400);

            DataContext = this;

            // Загружаем вкладки (с ограниченным функционалом)
            ProductsFrame.Navigate(new Guest.GuestPage(_mainWindow)); // Временно используем GuestPage
            // OrdersFrame можно загрузить аналогично
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
