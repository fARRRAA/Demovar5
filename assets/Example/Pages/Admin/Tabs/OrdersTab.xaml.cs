using BeautySalonWpf.WindowDialogs.AdminPage.Order;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeautySalonWpf.Pages.Admin.Tabs
{
    /// <summary>
    /// Логика взаимодействия для OrdersTab.xaml
    /// </summary>
    public partial class OrdersTab : Page
    {
        private List<Orders> _orders;
        private List<Orders> _allOrders;
        private int pageCount;
        private int pageSize = 9;
        public OrdersTab()
        {
            InitializeComponent();
            _orders = ConnectionDb.db.Orders.ToList();
            OrdersList.ItemsSource = _orders.Take(pageSize);
            _allOrders = _orders;
            pageCount = (int)Math.Round(Convert.ToDouble(_orders.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            AllTImeRadioBtn.IsChecked = true;
            
         
        }
        public async void UpdateOrdersList()
        {
            var index = paginationElem.PageIndex;
            var newItems = _orders = ConnectionDb.db.Orders.ToList();
            OrdersList.ItemsSource = newItems.Skip((paginationElem.PageIndex - 1) * pageSize).Take(pageSize).ToList();
            _orders = newItems;
            pageCount = (int)Math.Round(Convert.ToDouble(_orders.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            paginationElem.PageIndex = index;
        }
        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            OrdersList.ItemsSource = _orders.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }
        private void DateText_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateText.SelectedDate == null)
            {
                OrdersList.ItemsSource = _allOrders;
                return;
            }
            var Date = DateText.SelectedDate;
            var filtered = _orders.Where(a => a.date == Date).ToList();
            OrdersList.ItemsSource = filtered;
        }
        private void AppointmentSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = AppointmentSearchText.Text.ToLower();
            var filtered = _orders.Where(a => a.Clients.FName.ToLower().Contains(searchText)
            || a.Clients.Lname.ToLower().Contains(searchText)
           || a.Clients.FName.ToLower().Contains(searchText)
           || a.Clients.Lname.ToLower().Contains(searchText)
           || a.OrderStatus.name.ToLower().Contains(searchText)
            ).ToList();
            OrdersList.ItemsSource = filtered;
            //filtered[0].;
        }
        private void CancelFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            OrdersList.ItemsSource = _allOrders;
            AppointmentSearchText.Text = string.Empty;
            DateText.SelectedDate = null;
            AllTImeRadioBtn.IsChecked = true;

        }
        private void IsCompletedBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.IsChecked == true)
            {
                // Отменяем событие (т.е. не даем пользователю переключить RadioButton).
                e.Handled = true;
            }
        }
        private void AllTImeRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            OrdersList.ItemsSource = _allOrders;
        }

        private void TodayRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            var all = _orders.Where(x => x.date.Value.Day == DateTime.Now.Day
            && x.date.Value.Month == DateTime.Now.Month
            && x.date.Value.Year == DateTime.Now.Year
            ).ToList();
            OrdersList.ItemsSource = all;

        }

        private void ThisMonthRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            var all = _orders.Where(x => x.date.Value.Month == DateTime.Now.Month && x.date.Value.Year == DateTime.Now.Year).ToList();
            OrdersList.ItemsSource = all;
        }

        private async void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersList.SelectedItem as Orders;
           
            if (selected != null)
            {
                var order = ConnectionDb.db.Orders.FirstOrDefault(x => x.id == selected.id);
                var orderProducts =ConnectionDb.db.OrderProducts.Where(x=>x.orderId==order.id);
                foreach(var item in orderProducts)
                {
                    //var product=ConnectionDb.db.Products.FirstOrDefault(x=>x.productId==item.productId);
                    item.Products.inStock += item.count;
                    ConnectionDb.db.OrderProducts.Remove(item);
                }
                ConnectionDb.db.Orders.Remove(order);
                ConnectionDb.db.SaveChanges();
                UpdateOrdersList();
                Growl.Success("Заказ был удален");
                await Task.Delay(2000);
                Growl.Clear();
            }
        }

        private void RedactOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            var addOrderWindow = new AddOrder(this
                );
            addOrderWindow.Show();
        }

        private void OrdersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = OrdersList.SelectedItem as Orders;
            var viewOrderDialog = new ViewOrder(selected,this);
            viewOrderDialog.Show();
        }

        private void updateProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrdersList();
        }
    }
}
