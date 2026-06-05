using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Demovar5.Pages.Guest
{
    /// <summary>
    /// Логика взаимодействия для GuestPage.xaml
    /// </summary>
    public partial class GuestPage : Page
    {
        private MainWindow _mainWindow;
        private List<ProductViewModel> _products;

        public string TotalProductsText => $"Всего товаров: {_products?.Count ?? 0}";

        public GuestPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _mainWindow.ChangeWindowSize(768, 1024);
            
            DataContext = this;
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    _products = context.Products
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

                    ProductsItemsControl.ItemsSource = _products;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new SignInPage(_mainWindow));
        }
    }

    /// <summary>
    /// ViewModel для отображения товара
    /// </summary>
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ArticleNumber { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int QuantityInStock { get; set; }
        public string PhotoPath { get; set; }
        public string CategoryName { get; set; }
        public string ManufacturerName { get; set; }
        public string SupplierName { get; set; }
        public string UnitName { get; set; }

        public string PhotoFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(PhotoPath))
                    return "/Resources/Images/picture.png";

                var path = $"/Resources/Images/{PhotoPath}";
                return path;
            }
        }

        public decimal PriceWithDiscount
        {
            get
            {
                if (Discount > 0)
                    return Price * (1 - Discount / 100m);
                return Price;
            }
        }

        public string DiscountText
        {
            get
            {
                if (Discount > 0)
                    return $"Скидка {Discount}%";
                return string.Empty;
            }
        }

        public bool HasHighDiscount => Discount > 15;
        public bool IsOutOfStock => QuantityInStock == 0;
    }
}
