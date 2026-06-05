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
    /// Логика взаимодействия для AddProvider.xaml
    /// </summary>
    public partial class AddProvider : System.Windows.Window
    {
        private ProviderTab _owner;
        private BeautySalonWpf.Provider _provider = new BeautySalonWpf.Provider();
        public AddProvider(ProviderTab owner)
        {
            InitializeComponent();
            _owner = owner;
        }

        private async void ConfirmAddBtn_Click(object sender, RoutedEventArgs e)
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

            var provider = await ConnectionDb.db.Provider.FirstOrDefaultAsync(a => a.name == NameText.Text && a.INN == INNText.Text);
            if (provider != null)
            {
                Growl.Error("Поставщик с такими данными уже существует.");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            try
            {
                _provider.name = NameText.Text;
                _provider.phone = PhoneText.Text;
                _provider.INN = INNText.Text;

                ConnectionDb.db.Provider.Add(_provider);
                ConnectionDb.db.SaveChanges();
                Growl.Success("Добавление прошло успешно");
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
        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 