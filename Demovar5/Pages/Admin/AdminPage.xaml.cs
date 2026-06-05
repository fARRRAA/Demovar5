using System.Windows;
using System.Windows.Controls;
using Demovar5.Pages.Admin.Tabs;

namespace Demovar5.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private MainWindow _mainWindow;
        private Users _currentUser;

        public string UserFullName => _currentUser?.FullName ?? "Администратор";

        public AdminPage(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = user;
            _mainWindow.ChangeWindowSize(900, 1400);

            DataContext = this;

            // Загружаем вкладки
            ProductsFrame.Navigate(new ProductsTab(_mainWindow, _currentUser));
            OrdersFrame.Navigate(new OrdersTab(_mainWindow, _currentUser));
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
