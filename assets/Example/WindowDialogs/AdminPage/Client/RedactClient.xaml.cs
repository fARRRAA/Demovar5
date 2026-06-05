using BeautySalonWpf.Pages.Admin.Tabs;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BeautySalonWpf.WindowDialogs.Client
{
    /// <summary>
    /// Логика взаимодействия для RedactClient.xaml
    /// </summary>
    public partial class RedactClient : System.Windows.Window
    {
        private ClientsTab _owner;
        private string adminProfilePhotoFolder = "C:\\Users\\Ильдар\\source\\repos\\BeautySalonWpf\\BeautySalonWpf\\imgs\\pfp\\masters\\".Replace("\\", "/");
        string removePath = "/imgs/pfp/admins/";
        private Clients _client;
        public RedactClient(ClientsTab owner, Clients client)
        {
            InitializeComponent();
            _owner = owner;
            _client = client;
            FillFieldsWInfo();
        }

        private void FillFieldsWInfo()
        {
            FNameText.Text = _client.FName;
            LNameText.Text = _client.Lname;
            DateBirthText.SelectedDate = _client.dateBirth;
            PhoneText.Text = _client.phone;
            EmailText.Text = _client.email;
            LoginText.Text = _client.login;
            PreferencesText.Text = _client.Preferences;
            if (!string.IsNullOrWhiteSpace(_client.photo))
            {
                //Photo.Source = new BitmapImage(new Uri(System.IO.Path.Combine(adminProfilePhotoFolder, _client.photo.Replace(removePath, string.Empty)), UriKind.RelativeOrAbsolute));
            }
        }
        private async void ConfirmAddBtn_Click(object sender, RoutedEventArgs e)
        {
            var inputs = new[]
             {
        LoginText.Text,
        FNameText.Text,
        LNameText.Text,
        PhoneText.Text,
        EmailText.Text,
        PasswordText.Password,
                };

            if (inputs.Any(string.IsNullOrWhiteSpace) || !DateBirthText.SelectedDate.HasValue)
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var check = await ConnectionDb.db.Clients.FirstOrDefaultAsync(a => a.login == LoginText.Text && a.userID != _client.userID);
            if (check != null)
            {
                Growl.Error("Клиент с таким логином существует.");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var client = await ConnectionDb.db.Clients.FirstOrDefaultAsync(a => a.login == _client.login);

            client.FName = FNameText.Text;
            client.Lname = LNameText.Text;
            client.email = EmailText.Text;
            client.password = PasswordText.Password;
            client.dateBirth = DateBirthText.SelectedDate;
            client.phone = PhoneText.Text;
            client.login = LoginText.Text;
            client.Preferences= PreferencesText.Text;
            await ConnectionDb.db.SaveChangesAsync();
            Growl.Success("Редактирование прошло успешно");
            await Task.Delay(1000);
            Growl.Clear();
            _owner.UpdateClientsList();
            this.Close();
        }

        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addPhoto_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
