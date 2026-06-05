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

namespace BeautySalonWpf.WindowDialogs.AdminPage.Service
{
    /// <summary>
    /// Логика взаимодействия для RedactService.xaml
    /// </summary>
    public partial class RedactService : System.Windows.Window
    {
        private ServicesTab _owner;
        private List<TypeServices> _typeServices;
        private Services _service;
        public RedactService(ServicesTab owner, Services service)
        {
            InitializeComponent();
            _owner = owner;
            _typeServices = ConnectionDb.db.TypeServices.ToList();
            TypeService.ItemsSource = _typeServices.Select(s => s.name);
            _service = service;
            NameText.Text = _service.serviceName;
            TypeService.SelectedItem = _service.TypeServices.name;
            JuniorMoney.Value = (double)_service.juniorPrice;
            JuniorTime.Value = (double)_service.juniorRunTime;
            MiddleMoney.Value = (double)_service.middlePrice;
            MiddleTime.Value = (double)_service.middleRunTime;
            SeniorMoney.Value = (double)_service.seniorPrice;
            SeniorTime.Value = (double)_service.SeniorRunTime;
        }

        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            var service = await ConnectionDb.db.Services.FirstOrDefaultAsync(s => s.serviceId == _service.serviceId);
            if (service == null)
            {
                Growl.Error("fatal error");
                await Task.Delay(1000);
                Growl.Clear();
                this.Close();
                return;
            }
            service.serviceName = NameText.Text;
            service.juniorPrice = (int)JuniorMoney.Value;
            service.juniorRunTime = (int)JuniorTime.Value;
            service.middlePrice = (int)MiddleMoney.Value;
            service.middleRunTime = (int)MiddleTime.Value;
            service.seniorPrice = (int)SeniorMoney.Value;
            service.SeniorRunTime = (int)SeniorTime.Value;
            service.TypeServices = _typeServices.FirstOrDefault(s => s.name == TypeService.SelectedItem.ToString());
            ConnectionDb.db.SaveChanges();
            Growl.Success("Редактирование прошло успешно");
            await Task.Delay(1000);
            Growl.Clear();
            _owner.UpdateServicesList();
            this.Close();
        }
    }
}
