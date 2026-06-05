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

namespace BeautySalonWpf.WindowDialogs.AdminPage.DeliveryDialogs
{
    /// <summary>
    /// Логика взаимодействия для AddDelivery.xaml
    /// </summary>
    public partial class AddDelivery : System.Windows.Window
    {
        private DeliveryTab _owner;
        private List<BeautySalonWpf.Products> _products;
        private List<BeautySalonWpf.Provider> _providers;
        public AddDelivery(DeliveryTab owner)
        {
            InitializeComponent();
            _owner = owner;
            _products= ConnectionDb.db.Products.ToList();
            _providers=ConnectionDb.db.Provider.ToList();
            ProviderText.ItemsSource = _providers.Select(p=>p.name);
            ProductText.ItemsSource = _products.Select(p => p.name);
        }

        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ConfirmAddBtn_Click(object sender, RoutedEventArgs e)
        {
            var inputs = new[]
            {
                ProviderText.SelectedItem.ToString(),
                ProductText.SelectedItem.ToString(),
                Convert.ToString(CountText.Value),
                DateText.SelectedDate.ToString()
            };
            if (inputs.Any(string.IsNullOrWhiteSpace) || !DateText.SelectedDate.HasValue)
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            if (CountText.Value < 0)
            {
                Growl.Error("кол-во не может быть больше нуля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var delivery = new Delivery()
            {
                date= DateText.SelectedDate,
                count= (int?)CountText.Value,
                Products = _products.FirstOrDefault(p=>p.name == ProductText.SelectedItem.ToString()),
                Provider = _providers.FirstOrDefault(p=>p.name == ProviderText.SelectedItem.ToString())
            };
            ConnectionDb.db.Delivery.Add(delivery);
            var product = await ConnectionDb.db.Products.FirstOrDefaultAsync(p=>p.name == ProductText.SelectedItem.ToString());
            product.inStock += delivery.count;
            ConnectionDb.db.SaveChanges();
            Growl.Success("Редактирование прошло успешно");
            await Task.Delay(1000);
            Growl.Clear();
            _owner.UpdateDeliveryList();
            this.Close();
        }
    }
}
