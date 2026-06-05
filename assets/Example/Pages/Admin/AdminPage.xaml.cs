using BeautySalonWpf.Pages.Admin.Tabs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Net;
using System.Net.Mail;
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System.Drawing;
using HandyControl.Tools;
using System.Linq;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace BeautySalonWpf.Pages.Admin
{


    public partial class AdminPage : Page
    {
        private bool _isDarkTheme = true;
        private MainWindow _mw;
        private Admins _admin;
        private List<Admins> _admins;
        private List<string> choosedTypes = new List<string>();
        private static readonly HttpClient httpClient = new HttpClient();
        private const string ApiKey = "CF3044CE-B265-C317-6797-EF5C86BAC6BD"; // Укажите ваш API-ключ
        public AdminPage(MainWindow mw, Admins admin)
        {
            InitializeComponent();
            _mw = mw;
            _mw.ChangeWindowSize(950, 1450);
            _admin = admin;
            AdminTabFrame.Navigate(new AdminTab(_admin));
            MastersTabFrame.Navigate(new MastersTab(_admin));
            ClientsTabFrame.Navigate(new ClientsTab(_admin));
            ProductsFrame.Navigate(new ProductsTab(_admin));
            DeliveryTabFrame.Navigate(new DeliveryTab());
            ProvidersTabFrame.Navigate(new ProviderTab());
            ServicesTabFrame.Navigate(new ServicesTab());
            AppointmentsFrame.Navigate(new AppointmentsTab());
            OrdersFrame.Navigate(new OrdersTab());
           SalariesFrame.Navigate(new SalariesTab());
        }
       
        public void MasterStartSettings()
        {

        }
        private async void addPhoto_Click(object sender, RoutedEventArgs e)
        {
            string fileFolder = "C:\\Users\\Ильдар\\source\\repos\\BeautySalonWpf\\BeautySalonWpf\\docs\\".Replace("\\", "/");

            var fileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*jpeg;*img;*jpg;*png"
            };
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }
            if (fileDialog.ShowDialog() == true)
            {
                var filename = System.IO.Path.GetFileName(fileDialog.FileName);
                var path = System.IO.Path.Combine(fileFolder, filename);
                File.Copy(fileDialog.FileName, path, overwrite: true);
                Photo.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            }
        }
        void createreport()
        {

            string fileFolder = "C:\\Users\\Ильдар\\source\\repos\\BeautySalonWpf\\BeautySalonWpf\\docs\\".Replace("\\", "/");
            string text = "Привет, это пример текста для записи в файл!";

            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }
            var filepath = System.IO.Path.Combine(fileFolder, "report1.txt");
            File.WriteAllText(filepath, text);


        }
        //ot salonwpf gZwrWpzVeyS15wEy0x3v 
        void Send()
        {
            var server = "smtp.gmail.com";
            var port = 587;
            var user = "farrahovi2006@gmail.com";

            using (SmtpClient client = new SmtpClient(server, port))
            {
                client.Credentials = new NetworkCredential(user, "tmaq iswu pwov lpdi");
                client.EnableSsl = true;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(user);
                    mail.To.Add("farrahovildar1112@mail.ru");
                    mail.Subject = "dsds";
                    mail.Body = "sdsd";
                    client.Send(mail);
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            createreport();
            //var json = File.ReadAllText("C:\\Users\\Ильдар\\source\\repos\\BeautySalonWpf\\BeautySalonWpf\\secrets.json").Replace("\\", "/");
            //var secrets = JObject.Parse(json);
            //string smtpPassword = secrets["SmtpPassword"].ToString();

            //string smtpServer = "smtp.gmail.com";
            //int smtpPort = 587;
            //string smtpUsername = "farrahovi2006@gmail.com";

            //using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            //{
            //    smtpClient.Credentials = new NetworkCredential(smtpUsername, "tmaq iswu pwov lpdi");
            //    smtpClient.EnableSsl = true;

            //    using (MailMessage mailMessage = new MailMessage())
            //    {
            //        mailMessage.From = new MailAddress(smtpUsername);
            //        mailMessage.To.Add("farrahovildar1112@mail.ru");
            //        mailMessage.Subject = "Данные от входа в ИС мероприятий";
            //        mailMessage.Body = $"Логин: dsdsd \r\nПароль: sdsdd";
            //        try
            //        {
            //            smtpClient.Send(mailMessage);
            //            Growl.Success("Сообщение успешно отправлено.");
            //        }
            //        catch (Exception ex)
            //        {
            //            Growl.Error($"Ошибка отправки сообщения: {ex.Message}");
            //        }
            //    }
            //}

        }

        private async void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(250);
            _mw.MainFrame.Navigate(new SignIn(_mw));




        }

        /// <summary>
        /// Обработчик клика по кнопке смены темы
        /// </summary>
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
            var contentFrame = AdminPageGrid;
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


        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    string phoneNumber = "79393844323"; 
        //    string message = "Миша лох"; 

        //    await SendSmsAsync(phoneNumber, message);

        //}
        //static async Task SendSmsAsync(string phone, string message)
        //{
        //    string url = $"https://sms.ru/sms/send?api_id={ApiKey}&to={phone}&msg={Uri.EscapeDataString(message)}&json=1";

        //    var response = await httpClient.GetAsync(url);
        //        response.EnsureSuccessStatusCode();

        //        var responseBody = await response.Content.ReadAsStringAsync();
        //    Growl.Success($"{responseBody}");
        //}
    }
}
