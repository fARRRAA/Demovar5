using BeautySalonWpf.Pages.Admin.Tabs;
using HandyControl.Controls;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BeautySalonWpf.WindowDialogs.Provider
{
    /// <summary>
    /// Логика взаимодействия для RedactProvider.xaml
    /// </summary>
    public partial class RedactProvider : System.Windows.Window
    {
        private ProviderTab _owner;
        private BeautySalonWpf.Provider _provider;
        public RedactProvider(ProviderTab owner, BeautySalonWpf.Provider provider)
        {
            InitializeComponent();
            _owner = owner;
            _provider = provider;
            NameText.Text = _provider.name;
            PhoneText.Text = _provider.phone;
            INNText.Text = _provider.INN;
        }

        private async void ConfirmRedactBtn_Click(object sender, RoutedEventArgs e)
        {
            var inputs = new[]
            {
                NameText.Text,
                PhoneText.Text,
                INNText.Text
            };

            if (inputs.Any(string.IsNullOrWhiteSpace))
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }

            try
            {
                _provider.name = NameText.Text;
                _provider.phone = PhoneText.Text;
                _provider.INN = INNText.Text;

                ConnectionDb.db.Entry(_provider).State = EntityState.Modified;
                ConnectionDb.db.SaveChanges();
                Growl.Success("Изменения сохранены успешно");
                await Task.Delay(1000);
                Growl.Clear();
                _owner.UpdateProvidersList();
                this.Close();
            }
            catch (Exception ex)
            {
                Growl.Error($"Произошла ошибка: {ex.Message}");
            }
        }
        private void CloseRedactBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 