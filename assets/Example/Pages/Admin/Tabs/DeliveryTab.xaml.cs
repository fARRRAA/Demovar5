using BeautySalonWpf.WindowDialogs.AdminPage.DeliveryDialogs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeautySalonWpf.Pages.Admin.Tabs
{
    /// <summary>
    /// Логика взаимодействия для DeliveryTav.xaml
    /// </summary>
    public partial class DeliveryTab : Page
    {
        private List<Delivery> _deliveries;
        private int pageCount;
        private int pageSize = 12;
        public DeliveryTab()
        {
            InitializeComponent();
            _deliveries = ConnectionDb.db.Delivery.ToList();
            DeliveryList.ItemsSource = _deliveries;
            pageCount = (int)Math.Round(Convert.ToDouble(_deliveries.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            DeliveriesCountText.Content = $"Всего поставок: {_deliveries.Count}";
        }
        private void DeliverySearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = DeliverySearchText.Text.ToLower();
            var filtered = _deliveries.Where(a => a.Provider.name.ToLower().Contains(searchText)
            || a.Products.name.ToLower().Contains(searchText)
            ).ToList();
            DeliveryList.ItemsSource = filtered;
        }

        private async void deleteDelivery_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveryList.SelectedItems == null)
            {
                Growl.Error("выберите поставку");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var selectedDelivery = DeliveryList.SelectedItem as Delivery;
            var product = await ConnectionDb.db.Products.FirstOrDefaultAsync(p => p.productId == selectedDelivery.productId);
            product.inStock -= selectedDelivery.count;
            ConnectionDb.db.Delivery.Remove(selectedDelivery);
            ConnectionDb.db.SaveChanges();
            UpdateDeliveryList();
            Growl.Success("Удаление прошло успешно");
            await Task.Delay(1500);
            Growl.Clear();
        }

        private async void redactDelivery_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveryList.SelectedItems == null)
            {
                Growl.Error("выберите поставку");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var selectedDelivery = DeliveryList.SelectedItem as Delivery;
            var redactDeliveryDialog = new RedactDelivery(this, selectedDelivery);
            redactDeliveryDialog.Show();
        }

        private void addDelivery_Click(object sender, RoutedEventArgs e)
        {
            var AddDeliveryDialog = new AddDelivery(this);
            AddDeliveryDialog.Show();
        }
        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            DeliveryList.ItemsSource = _deliveries.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }
        public async void UpdateDeliveryList()
        {
            var newItems = await ConnectionDb.db.Delivery.ToListAsync();
            DeliveryList.ItemsSource = newItems.Skip((paginationElem.PageIndex-1) * pageSize).Take(pageSize).ToList();
            _deliveries = newItems;
            DeliveriesCountText.Content = $"Всего поставок: {newItems.Count}";
            pageCount = (int)Math.Round(Convert.ToDouble(_deliveries.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            paginationElem.PageIndex = pageCount;

        }

        private void DeliverySearchText_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void updateProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateDeliveryList();
        }
    }
}
