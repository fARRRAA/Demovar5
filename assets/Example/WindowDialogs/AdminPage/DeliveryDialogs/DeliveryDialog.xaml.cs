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
    /// Логика взаимодействия для DeliveryDialog.xaml
    /// </summary>
    public partial class RedactDelivery : System.Windows.Window
    {
        private DeliveryTab _owner;
        private Delivery _delivery;
        private List<BeautySalonWpf.Products> _products;
        private List<BeautySalonWpf.Provider> _providers;
        public RedactDelivery(DeliveryTab owner,Delivery delivery)
        {
            InitializeComponent();
            _owner = owner;
            _delivery = delivery;
            _products = ConnectionDb.db.Products.ToList();
            _providers = ConnectionDb.db.Provider.ToList();
            ProviderText.ItemsSource = _providers.Select(p => p.name);
            ProductText.ItemsSource = _products.Select(p => p.name);
            CountText.Value = (double)_delivery.count;
            DateText.SelectedDate = _delivery.date;
            ProductText.SelectedItem = _delivery.Products.name;
            ProviderText.SelectedItem=_delivery.Provider.name;
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
            var prevCount = _delivery.count;
            var delivery = await ConnectionDb.db.Delivery.FirstOrDefaultAsync(d=>d.deliveryId==_delivery.deliveryId);
            delivery.count -= _delivery.count;
            delivery.count +=(int) CountText.Value;
            delivery.Products =  _products.FirstOrDefault(p => p.name == ProductText.SelectedItem.ToString());
            delivery.Provider = _providers.FirstOrDefault(p => p.name == ProviderText.SelectedItem.ToString());

            var product = await ConnectionDb.db.Products.FirstOrDefaultAsync(p => p.productId == delivery.productId);
            product.inStock -= prevCount;
            product.inStock += (int)CountText.Value;
            ConnectionDb.db.SaveChanges();
            Growl.Success("Редактирование прошло успешно");
            await Task.Delay(1000);
            Growl.Clear();
            _owner.UpdateDeliveryList();
            this.Close();
        }

        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
