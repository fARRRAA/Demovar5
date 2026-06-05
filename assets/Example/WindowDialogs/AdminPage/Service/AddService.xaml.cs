using BeautySalonWpf.Pages.Admin.Tabs;
using HandyControl.Controls;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BeautySalonWpf.WindowDialogs.AdminPage.Service
{
    /// <summary>
    /// Логика взаимодействия для AddService.xaml
    /// </summary>
    public partial class AddService : System.Windows.Window
    {
        private ServicesTab _owner;
        private List<TypeServices> _typeServices;
        public AddService(ServicesTab owner)
        {
            InitializeComponent();
            _owner = owner;
            _typeServices = ConnectionDb.db.TypeServices.ToList();
            TypeService.ItemsSource = _typeServices.Select(s=>s.name);
        }

        private async void ConfirmAddBtn_Click(object sender, RoutedEventArgs e)
        {
            var inputs = new[]
            {
                NameText.Text,
                JuniorMoney.Value.ToString(),
                JuniorTime.Value.ToString(),
                MiddleMoney.Value.ToString(),
                MiddleTime.Value.ToString(),
                SeniorMoney.Value.ToString(),
                SeniorTime.Value.ToString(),
                TypeService.SelectedItem.ToString()
            };
            if (inputs.Any(string.IsNullOrWhiteSpace))
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var service = new Services()
            {
                serviceName = NameText.Text,
                juniorPrice = (int)JuniorMoney.Value,
                juniorRunTime = (int)JuniorTime.Value,
                middlePrice = (int)MiddleMoney.Value,
                middleRunTime = (int)MiddleTime.Value,
                seniorPrice = (int)SeniorMoney.Value,
                SeniorRunTime = (int)SeniorTime.Value,
                TypeServices = _typeServices.FirstOrDefault(s => s.name == TypeService.SelectedItem.ToString())
            };
            ConnectionDb.db.Services.Add(service);
            ConnectionDb.db.SaveChanges();
            Growl.Success("Добавление прошло успешно");
            await Task.Delay(1000);
            Growl.Clear();
            _owner.UpdateServicesList();
            this.Close();
        }

        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
