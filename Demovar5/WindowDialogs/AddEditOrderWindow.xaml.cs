using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using HandyControl.Controls;

namespace Demovar5.WindowDialogs
{
    /// <summary>
    /// Универсальное окно для добавления и редактирования заказа
    /// </summary>
    public partial class AddEditOrderWindow : Window, INotifyPropertyChanged
    {
        private Orders _order;
        private bool _isEditMode;
        private List<OrderProductItem> _orderProducts;

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

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(WindowTitle));
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }

        public string WindowTitle => IsEditMode ? "Редактирование заказа" : "Добавление заказа";
        public string SaveButtonText => IsEditMode ? "Сохранить" : "Добавить";

        public int TotalProductsCount => _orderProducts?.Sum(op => op.Quantity) ?? 0;
        public decimal TotalPrice
        {
            get
            {
                if (_orderProducts == null) return 0;
                
                decimal total = 0;
                using (var context = new demovar5Entities())
                {
                    foreach (var item in _orderProducts)
                    {
                        var product = context.Products.Find(item.ProductID);
                        if (product != null)
                        {
                            var price = product.Price * (1 - product.Discount / 100m);
                            total += price * item.Quantity;
                        }
                    }
                }
                return total;
            }
        }

        // Конструктор для добавления нового заказа
        public AddEditOrderWindow()
        {
            InitializeComponent();
            IsEditMode = false;
            InitializeNewOrder();
            LoadComboBoxes();
            DataContext = this;
        }

        // Конструктор для редактирования существующего заказа
        public AddEditOrderWindow(Orders order)
        {
            InitializeComponent();
            IsEditMode = true;
            Order = order;
            LoadComboBoxes();
            LoadOrderData();
            DataContext = this;
        }

        private void InitializeNewOrder()
        {
            Order = new Orders
            {
                OrderNumber = GenerateOrderNumber(),
                OrderDate = DateTime.Now,
                DeliveryDate = DateTime.Now.AddDays(7),
                ClientFullName = "",
                PickupCode = GeneratePickupCode(),
                OrderStatus = "Новый"
            };

            _orderProducts = new List<OrderProductItem>();
            OrderProductsListView.ItemsSource = _orderProducts;
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    // Загружаем пункты выдачи
                    PickupPointComboBox.ItemsSource = context.PickupPoints.ToList();
                }

                // Устанавливаем статус
                if (!IsEditMode)
                {
                    StatusComboBox.SelectedIndex = 0; // "Новый"
                }
                else
                {
                    StatusComboBox.SelectedItem = StatusComboBox.Items
                        .Cast<System.Windows.Controls.ComboBoxItem>()
                        .FirstOrDefault(item => item.Content.ToString() == Order.OrderStatus);
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadOrderData()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    _orderProducts = context.OrderProducts
                        .Where(op => op.OrderID == Order.OrderID)
                        .Select(op => new OrderProductItem
                        {
                            OrderProductID = op.OrderProductID,
                            ProductID = op.ProductID,
                            ProductName = op.Products.ProductName,
                            Quantity = op.Quantity
                        })
                        .ToList();

                    OrderProductsListView.ItemsSource = _orderProducts;
                    UpdateTotals();
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки состава заказа: {ex.Message}");
            }
        }

        private int GenerateOrderNumber()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    var maxOrderNumber = context.Orders.Max(o => (int?)o.OrderNumber) ?? 0;
                    return maxOrderNumber + 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        private int GeneratePickupCode()
        {
            var random = new Random();
            return random.Next(100, 1000);
        }

        private void AddProductToOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectProductWindow = new SelectProductWindow();
            if (selectProductWindow.ShowDialog() == true)
            {
                var selectedProduct = selectProductWindow.SelectedProduct;
                var quantity = selectProductWindow.Quantity;

                // Проверяем, есть ли уже этот товар в заказе
                var existingItem = _orderProducts.FirstOrDefault(op => op.ProductID == selectedProduct.ProductID);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    _orderProducts.Add(new OrderProductItem
                    {
                        ProductID = selectedProduct.ProductID,
                        ProductName = selectedProduct.ProductName,
                        Quantity = quantity
                    });
                }

                OrderProductsListView.Items.Refresh();
                UpdateTotals();
            }
        }

        private void RemoveProductFromOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var item = button.Tag as OrderProductItem;

            if (item != null)
            {
                _orderProducts.Remove(item);
                OrderProductsListView.Items.Refresh();
                UpdateTotals();
            }
        }

        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(TotalProductsCount));
            OnPropertyChanged(nameof(TotalPrice));
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (!ValidateInput())
                return;

            try
            {
                using (var context = new demovar5Entities())
                {
                    if (IsEditMode)
                    {
                        // Редактирование существующего заказа
                        var existingOrder = context.Orders.Find(Order.OrderID);
                        if (existingOrder == null)
                        {
                            Growl.Error("Заказ не найден");
                            return;
                        }

                        // Обновляем данные заказа
                        existingOrder.OrderNumber = Order.OrderNumber;
                        existingOrder.OrderDate = Order.OrderDate;
                        existingOrder.DeliveryDate = Order.DeliveryDate;
                        existingOrder.PickupPointID = Order.PickupPointID;
                        existingOrder.ClientFullName = Order.ClientFullName;
                        existingOrder.PickupCode = Order.PickupCode;
                        existingOrder.OrderStatus = ((System.Windows.Controls.ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();

                        // Удаляем старый состав заказа
                        var oldOrderProducts = context.OrderProducts.Where(op => op.OrderID == Order.OrderID).ToList();
                        context.OrderProducts.RemoveRange(oldOrderProducts);

                        // Добавляем новый состав заказа
                        foreach (var item in _orderProducts)
                        {
                            context.OrderProducts.Add(new OrderProducts
                            {
                                OrderID = existingOrder.OrderID,
                                ProductID = item.ProductID,
                                Quantity = item.Quantity
                            });
                        }

                        context.SaveChanges();
                        Growl.Success("Заказ успешно обновлен");
                    }
                    else
                    {
                        // Добавление нового заказа
                        // Проверяем уникальность номера заказа
                        if (context.Orders.Any(o => o.OrderNumber == Order.OrderNumber))
                        {
                            Growl.Warning("Заказ с таким номером уже существует");
                            return;
                        }

                        Order.OrderStatus = ((System.Windows.Controls.ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();
                        context.Orders.Add(Order);
                        context.SaveChanges();

                        // Добавляем состав заказа
                        foreach (var item in _orderProducts)
                        {
                            context.OrderProducts.Add(new OrderProducts
                            {
                                OrderID = Order.OrderID,
                                ProductID = item.ProductID,
                                Quantity = item.Quantity
                            });
                        }

                        context.SaveChanges();
                        Growl.Success("Заказ успешно добавлен");
                    }

                    await System.Threading.Tasks.Task.Delay(1000);
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            if (Order.OrderNumber <= 0)
            {
                Growl.Warning("Введите корректный номер заказа");
                OrderNumberNumeric.Focus();
                return false;
            }

            if (Order.OrderDate == default(DateTime))
            {
                Growl.Warning("Выберите дату заказа");
                OrderDatePicker.Focus();
                return false;
            }

            if (Order.DeliveryDate == default(DateTime))
            {
                Growl.Warning("Выберите дату доставки");
                DeliveryDatePicker.Focus();
                return false;
            }

            if (Order.DeliveryDate < Order.OrderDate)
            {
                Growl.Warning("Дата доставки не может быть раньше даты заказа");
                DeliveryDatePicker.Focus();
                return false;
            }

            if (Order.PickupPointID == 0)
            {
                Growl.Warning("Выберите пункт выдачи");
                PickupPointComboBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Order.ClientFullName))
            {
                Growl.Warning("Введите ФИО клиента");
                ClientNameTextBox.Focus();
                return false;
            }

            if (Order.PickupCode < 100 || Order.PickupCode > 999)
            {
                Growl.Warning("Код получения должен быть трёхзначным числом (100-999)");
                PickupCodeNumeric.Focus();
                return false;
            }

            if (_orderProducts == null || _orderProducts.Count == 0)
            {
                Growl.Warning("Добавьте хотя бы один товар в заказ");
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Класс для хранения информации о товаре в заказе
    /// </summary>
    public class OrderProductItem
    {
        public int OrderProductID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
