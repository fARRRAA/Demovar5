using BeautySalonWpf.Pages.Admin;
using BeautySalonWpf.Pages.Master;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace BeautySalonWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : Page
    {
        private MainWindow _mw;
        private bool _isDarkTheme = true;
        public SignIn(MainWindow mw)
        {
            InitializeComponent();
            _mw = mw;
            _mw.ChangeWindowSize(600, 700);
            LoginTextAdmin.Text = "farra";
            PasswordTextAdmin.Password = "123";
            LoginTextMaster.Text = "fara";
            PasswordTextMaster.Password = "123";
            TabControl.SelectedIndex = 1;
            _isDarkTheme = true;
            App.SetTheme(_isDarkTheme);
            UpdateThemeButtonText();

        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            AnimateThemeWithScale( () => {
                _isDarkTheme = !_isDarkTheme;
                App.SetTheme(_isDarkTheme);

            });
        }

        private void AnimateThemeWithScale(Action themeChangeAction)
        {
            // Находим элемент Frame для анимации
            var contentFrame = SignInGrid;
            if (contentFrame != null)
            {
                // Создаем анимацию уменьшения масштаба
                ScaleTransform scaleTransform = new ScaleTransform(1, 1);
                contentFrame.RenderTransform = scaleTransform;
                contentFrame.RenderTransformOrigin = new Point(0.5, 0.5);

                DoubleAnimation scaleOutX = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.97,
                    Duration = TimeSpan.FromSeconds(0.15)
                };

                DoubleAnimation scaleOutY = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.97,
                    Duration = TimeSpan.FromSeconds(0.15)
                };

                // Анимация прозрачности
                DoubleAnimation fadeOut = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.5,
                    Duration = TimeSpan.FromSeconds(0.15)
                };

                // Назначаем обработчик завершения анимации
                fadeOut.Completed += (s, e) =>
                {
                    // Меняем тему во время анимации
                    themeChangeAction?.Invoke();

                    // Анимация увеличения масштаба
                    DoubleAnimation scaleInX = new DoubleAnimation
                    {
                        From = 0.97,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(0.15)
                    };

                    DoubleAnimation scaleInY = new DoubleAnimation
                    {
                        From = 0.97,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(0.15)
                    };

                    // Анимация прозрачности
                    DoubleAnimation fadeIn = new DoubleAnimation
                    {
                        From = 0.5,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(0.15)
                    };

                    // Запускаем анимации возврата
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleInX);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleInY);
                    contentFrame.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                // Запускаем анимацию
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleOutX);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleOutY);
                contentFrame.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
            else
            {
                // Если элемент не найден, просто меняем тему
                themeChangeAction?.Invoke();
            }
        }

        /// <summary>
        /// Обновляет текст кнопки смены темы в зависимости от текущей темы
        /// </summary>
        private void UpdateThemeButtonText()
        {
            if (ThemeToggleButton != null)
            {
                var textBlock = ((StackPanel)ThemeToggleButton.Content).Children.OfType<TextBlock>().FirstOrDefault();
                if (textBlock != null)
                {
                    textBlock.Text = _isDarkTheme ? "Светлая тема" : "Тёмная тема";
                }

                // Обновляем иконку в зависимости от темы
                var image = ((StackPanel)ThemeToggleButton.Content).Children.OfType<Image>().FirstOrDefault();
                if (image != null)
                {
                    image.Source = new BitmapImage(new Uri(_isDarkTheme ?
                        "/imgs/media/icons/lightmode.png" :
                        "/imgs/media/icons/darkmode.png", UriKind.Relative));
                }
            }
        }
        private void notRegistered_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(new SignUp(_mw));

        }

        private async void LoginBtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextAdmin.Text;
            var password = PasswordTextAdmin.Password;
            var temp = new Admins()
            {
                login = login,
                password = password,
            };
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var check =  ConnectionDb.db.Admins.FirstOrDefault(x => x.login == login && x.password == password);
            if (check == null)
            {
                Growl.Error("Неправильный логин или пароль");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }

            Growl.Success("Вы успешно вошли в систему");
            await Task.Delay(1500);
            Growl.Clear();
            _mw.MainFrame.NavigationService.Navigate(new AdminPage(_mw,check));
        }

        private async void LoginBtnMaster_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextMaster.Text;
            var password = PasswordTextMaster.Password;
            var temp = new Masters()
            {
                login = login,
                password = password,
            };
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var check = ConnectionDb.db.Masters.FirstOrDefault(x => x.login == login && x.password == password);
            if (check == null)
            {
                Growl.Error("Неправильный логин или пароль");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            Growl.Success("Вы успешно вошли в систему");
            await Task.Delay(1500);
            Growl.Clear();
            _mw.MainFrame.NavigationService.Navigate(new MasterPage(_mw,check));
        }
    }
}
