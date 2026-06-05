using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeautySalonWpf.Pages.Master
{
    /// <summary>
    /// Логика взаимодействия для MasterPage.xaml
    /// </summary>
    public partial class MasterPage : Page
    {
        private Masters _master;
        private MainWindow _mw;
        private bool _isDarkTheme = true;
        public MasterPage(MainWindow mw, Masters master)
        {
            InitializeComponent();
            _mw = mw;
            _mw.ChangeWindowSize(950, 1450);



            ClientsTabFrame.Navigate(new ClientPage());
            ServicesTabFrame.Navigate(new MasterServicesTab(master));
            ProductsFrame.Navigate(new MaterialsTab(master));
            AppointmentsFrame.Navigate(new MasterAppointments(master));
            SalaryFrame.Navigate(new SalaryPage(master));

            MasterFnameText.Text = master.Fname;
            MasterLnameText.Text = master.Lname;
            MasterEmailText.Text = master.email;
            MasterPatronymicText.Text = master.Patronymic;
            MasterSkillText.Text = master.MastersSkills.name;
            MasterPhoneText.Text = master.phone;
            MasterQualificationText.Text = master.MastersQualifications.TypeServices.name;
            _master = master;


            var allAppointments = ConnectionDb.db.Appointments.ToList();
            var countAppointments = ConnectionDb.db.Appointments.Count(a => a.masterId == _master.masterId);
            AllAppointmentsText.Content = countAppointments;
            var appointmentsInMonth = allAppointments.Count(a => a.date.Value.Month == DateTime.Now.Month && a.masterId == master.masterId);
            MonthAppoinmentsText.Content = appointmentsInMonth;
            var salary = ConnectionDb.db.MastersSalaries.FirstOrDefault(a => a.masterId == _master.masterId && a.year == DateTime.Now.Year && a.month == DateTime.Now.Month);
            //SalatyText.Content = $"{salary.salary} ₽";
        }

        private async void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(250);
            _mw.MainFrame.Navigate(new SignIn(_mw));
        }
        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            AnimateThemeWithScale(() => {
                _isDarkTheme = !_isDarkTheme;
                App.SetTheme(_isDarkTheme);

            });
        }

        private void AnimateThemeWithScale(Action themeChangeAction)
        {
            // Находим элемент Frame для анимации
            var contentFrame = MasrerPageGrid;
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
    }
}
