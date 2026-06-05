using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;
using Demovar5.Pages.Guest;

namespace Demovar5.Pages.Admin.Tabs
{
    /// <summary>
    /// Логика взаимодействия для ProductsTab.xaml
    /// </summary>
    public partial class ProductsTab : Page
    {
        private MainWindow _mainWindow;
        private Users _currentUser;
        private List<ProductViewModel> _allProducts;
        private List<ProductViewModel> _filteredProducts;

        public string TotalProductsText => $"Всего товаров: {_filteredProducts?.Count ?? 0}";

        public ProductsTab(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = user;
            DataContext = this;
            
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    _allProducts = context.Products
                        .Select(p => new ProductViewModel
                        {
                            ProductID = p.ProductID,
                            ArticleNumber = p.ArticleNumber,
                            ProductName = p.ProductName,
                            Description = p.Description,
                            Price = p.Price,
                            Discount = p.Discount,
                            QuantityInStock = p.QuantityInStock,
                            PhotoPath = p.PhotoPath,
                            CategoryName = p.Categories.CategoryName,
                            ManufacturerName = p.Manufacturers.ManufacturerName,
                            SupplierName = p.Suppliers.SupplierName,
                            UnitName = p.Units.UnitName
                        })
                        .ToList();

                    ApplyFiltersAndSort();
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private void ApplyFiltersAndSort()
        {
            _filteredProducts = new List<ProductViewModel>(_allProducts);

            // Применяем поиск
            var searchText = SearchBar.Text?.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                _filteredProducts = _filteredProducts.Where(p =>
                    p.ProductName.ToLower().Contains(searchText) ||
                    p.ArticleNumber.ToLower().Contains(searchText) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchText)) ||
                    p.CategoryName.ToLower().Contains(searchText) ||
                    p.ManufacturerName.ToLower().Contains(searchText) ||
                    p.SupplierName.ToLower().Contains(searchText)
                ).ToList();
            }

            // Применяем фильтр по скидке
            var discountFilterIndex = DiscountFilterComboBox.SelectedIndex;
            switch (discountFilterIndex)
            {
                case 1: // 0-10,99%
                    _filteredProducts = _filteredProducts.Where(p => p.Discount >= 0 && p.Discount < 11).ToList();
                    break;
                case 2: // 11-14,99%
                    _filteredProducts = _filteredProducts.Where(p => p.Discount >= 11 && p.Discount < 15).ToList();
                    break;
                case 3: // 15% и более
                    _filteredProducts = _filteredProducts.Where(p => p.Discount >= 15).ToList();
                    break;
            }

            // Применяем сортировку
            var sortIndex = SortComboBox.SelectedIndex;
            switch (sortIndex)
            {
                case 1: // Цена ↑
                    _filteredProducts = _filteredProducts.OrderBy(p => p.PriceWithDiscount).ToList();
                    break;
                case 2: // Цена ↓
                    _filteredProducts = _filteredProducts.OrderByDescending(p => p.PriceWithDiscount).ToList();
                    break;
                case 3: // Остаток ↑
                    _filteredProducts = _filteredProducts.OrderBy(p => p.QuantityInStock).ToList();
                    break;
                case 4: // Остаток ↓
                    _filteredProducts = _filteredProducts.OrderByDescending(p => p.QuantityInStock).ToList();
                    break;
            }

            ProductsItemsControl.ItemsSource = _filteredProducts;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void DiscountFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allProducts != null)
                ApplyFiltersAndSort();
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allProducts != null)
                ApplyFiltersAndSort();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowDialogs.AddEditProductWindow();
            if (window.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var productId = (int)button.Tag;

            try
            {
                using (var context = new demovar5Entities())
                {
                    var product = context.Products.Find(productId);
                    if (product == null)
                    {
                        Growl.Error("Товар не найден");
                        return;
                    }

                    var window = new WindowDialogs.AddEditProductWindow(product);
                    if (window.ShowDialog() == true)
                    {
                        LoadProducts();
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка: {ex.Message}");
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var productId = (int)button.Tag;

            var result = System.Windows.MessageBox.Show(
                "Вы уверены, что хотите удалить этот товар?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new demovar5Entities())
                    {
                        var product = context.Products.Find(productId);
                        
                        if (product == null)
                        {
                            Growl.Error("Товар не найден");
                            return;
                        }

                        // Проверяем, есть ли товар в заказах
                        var hasOrders = context.OrderProducts.Any(op => op.ProductID == productId);
                        if (hasOrders)
                        {
                            Growl.Warning("Товар присутствует в заказах. Удаление невозможно.");
                            return;
                        }

                        context.Products.Remove(product);
                        context.SaveChanges();
                        
                        Growl.Success("Товар успешно удален");
                        LoadProducts();
                    }
                }
                catch (Exception ex)
                {
                    Growl.Error($"Ошибка при удалении товара: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Расширенная ViewModel для товара с дополнительными свойствами для отображения
    /// </summary>
    public partial class ProductViewModel
    {
        public string PriceDisplay => PriceWithDiscount.ToString("N2");
        
        public string OriginalPriceDisplay => Discount > 0 ? Price.ToString("N2") + " руб." : "";
        
        public Visibility OriginalPriceVisibility => Discount > 0 ? Visibility.Visible : Visibility.Collapsed;
        
        public Brush PriceColor => Discount > 0 ? Brushes.Black : new SolidColorBrush(Color.FromRgb(0, 0, 255));
        
        public Brush BackgroundColor
        {
            get
            {
                if (IsOutOfStock)
                    return new SolidColorBrush(Color.FromRgb(200, 200, 200)); // Серый
                if (HasHighDiscount)
                    return new SolidColorBrush(Color.FromRgb(0, 128, 128)); // #008080
                return Brushes.White;
            }
        }
    }
}
