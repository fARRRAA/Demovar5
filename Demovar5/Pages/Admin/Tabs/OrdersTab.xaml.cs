using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;

namespace Demovar5.Pages.Admin.Tabs
{
    /// <summary>
    /// Логика взаимодействия для OrdersTab.xaml
    /// </summary>
    public partial class OrdersTab : Page
    {
        private MainWindow _mainWindow;
        private Users _currentUser;
        private List<OrderViewModel> _orders;

        public string TotalOrdersText => $"Всего заказов: {_orders?.Count ?? 0}";

        public OrdersTab(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = user;
            DataContext = this;
            
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    _orders = context.Orders
                        .Select(o => new OrderViewModel
                        {
                            OrderID = o.OrderID,
                            OrderNumber = o.OrderNumber,
                            OrderDate = o.OrderDate,
                            DeliveryDate = o.DeliveryDate,
                            ClientFullName = o.ClientFullName,
                            PickupCode = o.PickupCode,
                            OrderStatus = o.OrderStatus,
                            PickupPointAddress = o.PickupPoints.Address
                        })
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();

                    OrdersItemsControl.ItemsSource = _orders;
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки заказов: {ex.Message}");
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowDialogs.AddEditOrderWindow();
            if (window.ShowDialog() == true)
            {
                LoadOrders();
            }
        }

        private void ViewOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderId = (int)button.Tag;

            try
            {
                using (var context = new demovar5Entities())
                {
                    var order = context.Orders.Find(orderId);
                    if (order == null)
                    {
                        Growl.Error("Заказ не найден");
                        return;
                    }

                    var window = new WindowDialogs.ViewOrderWindow(order);
                    window.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка: {ex.Message}");
            }
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderId = (int)button.Tag;

            try
            {
                using (var context = new demovar5Entities())
                {
                    var order = context.Orders.Find(orderId);
                    if (order == null)
                    {
                        Growl.Error("Заказ не найден");
                        return;
                    }

                    var window = new WindowDialogs.AddEditOrderWindow(order);
                    if (window.ShowDialog() == true)
                    {
                        LoadOrders();
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка: {ex.Message}");
            }
        }

        private async void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderId = (int)button.Tag;

            var result = System.Windows.MessageBox.Show(
                "Вы уверены, что хотите удалить этот заказ?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new demovar5Entities())
                    {
                        var order = context.Orders.Find(orderId);
                        
                        if (order == null)
                        {
                            Growl.Error("Заказ не найден");
                            return;
                        }

                        context.Orders.Remove(order);
                        context.SaveChanges();
                        
                        Growl.Success("Заказ успешно удален");
                        LoadOrders();
                    }
                }
                catch (Exception ex)
                {
                    Growl.Error($"Ошибка при удалении заказа: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// ViewModel для отображения заказа
    /// </summary>
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string ClientFullName { get; set; }
        public int PickupCode { get; set; }
        public string OrderStatus { get; set; }
        public string PickupPointAddress { get; set; }

        public Brush StatusColor
        {
            get
            {
                return OrderStatus == "Завершен" 
                    ? Brushes.Green 
                    : new SolidColorBrush(Color.FromRgb(0, 0, 255));
            }
        }
    }
}
