using BeautySalonWpf.Pages.Admin.Tabs;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
    /// Логика взаимодействия для AddClient.xaml
    /// </summary>
    public partial class AddClient : System.Windows.Window
    {
        private ClientsTab _owner;
        private Clients _client = new Clients();
        public AddClient(ClientsTab owner)
        {
            InitializeComponent();
            _owner = owner;
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
        PasswordText.Password
                };

            if (inputs.Any(string.IsNullOrWhiteSpace) || !DateBirthText.SelectedDate.HasValue)
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }

            var client = await ConnectionDb.db.Clients.FirstOrDefaultAsync(a => a.login == LoginText.Text);
            if (client != null)
            {
                Growl.Error("Клиент с таким логином существует.");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            try
            {
                _client.FName = FNameText.Text;
                _client.Lname = LNameText.Text;
                _client.email = EmailText.Text;
                _client.password = PasswordText.Password;
                _client.dateBirth = DateBirthText.SelectedDate;
                _client.phone = PhoneText.Text;
                _client.login = LoginText.Text;
                _client.Preferences = PreferencesText.Text;

                _client.roleId = 3;
                ConnectionDb.db.Clients.Add(_client);
                ConnectionDb.db.SaveChanges();
                Growl.Success("Добавление прошло успешно");
                await Task.Delay(1000);
                Growl.Clear();
                _owner.UpdateClientsList();
                this.Close();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Growl.Error($"Свойство: {validationError.PropertyName}, Ошибка: {validationError.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Произошла ошибка: {ex.Message}");
            }
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