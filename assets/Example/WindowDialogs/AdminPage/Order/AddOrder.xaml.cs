using BeautySalonWpf.Pages.Admin.Tabs;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BeautySalonWpf.WindowDialogs.AdminPage.Order
{
    /// <summary>
    /// Логика взаимодействия для AddOrder.xaml
    /// </summary>
    public partial class AddOrder : System.Windows.Window
    {
        private OrdersTab _owner;
        private ObservableCollection<BeautySalonWpf.Products> availableProducts;
        private ObservableCollection<OrderProductItem> selectedProducts;
        private decimal totalPrice = 0;
        private List<Clients> _clients;
        public class OrderProductItem
        {
            public BeautySalonWpf.Products Product { get; set; }
            private int quantity;
            public int Quantity
            {
                get { return quantity; }
                set
                {
                    if (value >= 0)
                    {
                        quantity = value;
                    }
                }
            }
        }

        public AddOrder(OrdersTab owner)
        {
            InitializeComponent();
            _owner = owner;
            var productList = ConnectionDb.db.Products.Where(x=>x.inStock>0).ToList();

            availableProducts = new ObservableCollection<BeautySalonWpf.Products>(productList);
            selectedProducts = new ObservableCollection<OrderProductItem>();


            AvailableProductsListBox.ItemsSource = availableProducts;
            SelectedProductsListBox.ItemsSource = selectedProducts;

            OrderDate.SelectedDate = DateTime.Now;

            _clients=ConnectionDb.db.Clients.ToList();
            ClientsComboBox.ItemsSource = _clients.Select(x=>x.Lname+" "+x.FName);
        }




        private async void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = AvailableProductsListBox.SelectedItem as BeautySalonWpf.Products;
            if (selectedProduct != null)
            {
                if (selectedProduct.inStock <= 0)
                {
                    Growl.Warning("Товар закончился на складе!");
                    await Task.Delay(2000);
                    Growl.Clear();

                    return;
                }

                var existingItem = selectedProducts.FirstOrDefault(x => x.Product.productId == selectedProduct.productId);
                if (existingItem != null)
                {
                    if (existingItem.Quantity < selectedProduct.inStock)
                    {
                        existingItem.Quantity++;
                        UpdateTotalPrice();
                    }
                    else
                    {
                        Growl.Warning("Достигнуто максимальное количество товара!");
                        await Task.Delay(2000);
                        Growl.Clear();

                    }
                }
                else
                {
                    selectedProducts.Add(new OrderProductItem { Product = selectedProduct, Quantity = 1 });
                    UpdateTotalPrice();
                }
            }
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = SelectedProductsListBox.SelectedItem as OrderProductItem;
            if (selectedItem != null)
            {
                selectedProducts.Remove(selectedItem);
                UpdateTotalPrice();
            }
        }

        private void UpdateTotalPrice()
        {
            totalPrice = (int)selectedProducts.Sum(item => item.Product.price * item.Quantity);
            TotalPriceText.Text = $"Итого: {totalPrice} руб.";
        }

        private async void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            
            if (!e.Handled)
            {
                var textBox = sender as System.Windows.Controls.TextBox;
                var orderItem = (textBox.DataContext as OrderProductItem);
                if (orderItem != null)
                {
                    string newText = textBox.Text + e.Text;
                    if (int.TryParse(newText, out int newQuantity))
                    {
                        orderItem.Quantity = newQuantity;
                        UpdateTotalPrice();
                        if (newQuantity > orderItem.Product.inStock)
                        {
                            e.Handled = true;
                            Growl.Warning("Превышено доступное количество товара!");
                            await Task.Delay(2000);
                            Growl.Clear();
                        }
                    }
                }
            }
        }

        private  async void ConfirmAddBtn_Click(object sender, RoutedEventArgs e)
        {
            var discount = 0;
            if (ClientsComboBox.SelectedValue == null)
            {
                Growl.Warning("Превышено доступное количество товара!");
                await Task.Delay(2000);
                Growl.Clear(); return;
            }

            if (selectedProducts.Count == 0)
            {
                Growl.Warning("Превышено доступное количество товара!");
                await Task.Delay(2000);
                Growl.Clear(); return;
            }

            var order = new Orders()
            {
                Clients = _clients.FirstOrDefault(x => x.Lname + " " + x.FName == ClientsComboBox.SelectedItem.ToString()),
                date=OrderDate.SelectedDate,
                sum=(int)totalPrice,
                discount=0,
               discountSum= totalPrice-(totalPrice * (discount/100)),
               statusId=1,
               count=selectedProducts.Count
            };
            ConnectionDb.db.Orders.Add(order);
            ConnectionDb.db.SaveChanges();
            for(int i = 0; i < selectedProducts.Count; i++)
            {
                var orderItem = new OrderProducts()
                {
                    orderId = order.id,
                    Products = selectedProducts[i].Product,
                    count = selectedProducts[i].Quantity
                };
                ConnectionDb.db.OrderProducts.Add(orderItem);
                var product = ConnectionDb.db.Products.FirstOrDefault(x=>x.productId==orderItem.Products.productId);
                product.inStock -= orderItem.count;
            }
            foreach(var product in selectedProducts)
            {
                product.Product.soldCount += product.Quantity;
            }
            ConnectionDb.db.SaveChanges();
            Growl.Success("Заказ успешно создан");
            await Task.Delay(1000);
            Growl.Clear();
            _owner.UpdateOrdersList();
            this.Close();
        }

        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
