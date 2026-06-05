using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Demovar5.WindowDialogs
{
    /// <summary>
    /// Окно просмотра заказа
    /// </summary>
    public partial class ViewOrderWindow : Window, INotifyPropertyChanged
    {
        private Orders _order;
        private List<OrderProductDetail> _orderProducts;

        public event PropertyChangedEventHandler PropertyChanged;

        public Orders Order
        {
            get => _order;
            set
            {
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }

        public string StatusText => $"Статус: {Order.OrderStatus}";

        public Brush StatusColor
        {
            get
            {
                return Order.OrderStatus == "Завершен"
                    ? Brushes.Green
                    : new SolidColorBrush(Color.FromRgb(0, 0, 255));
            }
        }

        public string PickupPointAddress { get; set; }

        public decimal TotalPrice
        {
            get
            {
                return _orderProducts?.Sum(op => op.Total) ?? 0;
            }
        }

        public ViewOrderWindow(Orders order)
        {
            InitializeComponent();
            Order = order;
            LoadOrderDetails();
            DataContext = this;
        }

        private void LoadOrderDetails()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    // Загружаем адрес пункта выдачи
                    var pickupPoint = context.PickupPoints.Find(Order.PickupPointID);
                    PickupPointAddress = pickupPoint?.Address ?? "Не указан";

                    // Загружаем состав заказа
                    _orderProducts = context.OrderProducts
                        .Where(op => op.OrderID == Order.OrderID)
                        .Select(op => new OrderProductDetail
                        {
                            ArticleNumber = op.Products.ArticleNumber,
                            ProductName = op.Products.ProductName,
                            Price = op.Products.Price,
                            Discount = op.Products.Discount,
                            Quantity = op.Quantity
                        })
                        .ToList();

                    OrderProductsListView.ItemsSource = _orderProducts;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки данных заказа: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Детальная информация о товаре в заказе
    /// </summary>
    public class OrderProductDetail
    {
        public string ArticleNumber { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }

        public decimal PriceWithDiscount => Price * (1 - Discount / 100m);
        public decimal Total => PriceWithDiscount * Quantity;
    }
}
