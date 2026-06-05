using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;

namespace Demovar5.WindowDialogs
{
    /// <summary>
    /// Окно выбора товара для добавления в заказ
    /// </summary>
    public partial class SelectProductWindow : Window, INotifyPropertyChanged
    {
        private List<Products> _allProducts;
        private List<Products> _filteredProducts;
        private Products _selectedProduct;

        public event PropertyChangedEventHandler PropertyChanged;

        public Products SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
                OnPropertyChanged(nameof(IsProductSelected));
                OnPropertyChanged(nameof(SelectedProductInfo));
            }
        }

        public int Quantity => (int)QuantityNumeric.Value;

        public bool IsProductSelected => SelectedProduct != null;

        public string SelectedProductInfo
        {
            get
            {
                if (SelectedProduct == null)
                    return "Выберите товар из списка";

                var price = SelectedProduct.Price * (1 - SelectedProduct.Discount / 100m);
                var total = price * Quantity;
                return $"Цена со скидкой: {price:N2} руб. × {Quantity} = {total:N2} руб.";
            }
        }

        public SelectProductWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadProducts();

            // Обновляем информацию при изменении количества
            QuantityNumeric.ValueChanged += (s, e) => OnPropertyChanged(nameof(SelectedProductInfo));
        }

        private void LoadProducts()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    _allProducts = context.Products
                        .Where(p => p.QuantityInStock > 0) // Показываем только товары в наличии
                        .OrderBy(p => p.ProductName)
                        .ToList();

                    _filteredProducts = new List<Products>(_allProducts);
                    ProductsListView.ItemsSource = _filteredProducts;
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchBar.Text?.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredProducts = new List<Products>(_allProducts);
            }
            else
            {
                _filteredProducts = _allProducts.Where(p =>
                    p.ProductName.ToLower().Contains(searchText) ||
                    p.ArticleNumber.ToLower().Contains(searchText) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchText))
                ).ToList();
            }

            ProductsListView.ItemsSource = _filteredProducts;
        }

        private void ProductsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProduct = ProductsListView.SelectedItem as Products;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null)
            {
                Growl.Warning("Выберите товар из списка");
                return;
            }

            if (Quantity > SelectedProduct.QuantityInStock)
            {
                Growl.Warning($"На складе доступно только {SelectedProduct.QuantityInStock} шт.");
                return;
            }

            DialogResult = true;
            Close();
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
}
