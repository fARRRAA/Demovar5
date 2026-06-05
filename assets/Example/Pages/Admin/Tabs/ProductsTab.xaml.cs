using BeautySalonWpf.WindowDialogs.AdminPage.ProductDialog;
using BeautySalonWpf.WindowDialogs.AdminPage.Products;
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
    /// Логика взаимодействия для ProductsTab.xaml
    /// </summary>
    public partial class ProductsTab : Page
    {
        private Admins _admin;
        private List<Products> _products;
        private int pageCount;
        private int pageSize = 12;
        private List<TypeProducts> typeProducts = ConnectionDb.db.TypeProducts.ToList();

        public ProductsTab(Admins admin)
        {
            InitializeComponent();
            _admin = admin;
            _products = ConnectionDb.db.Products.ToList();
            pageCount = (int)Math.Round(Convert.ToDouble(_products.Count / pageSize)) + 1;
            ProductsList.ItemsSource = _products.Take(pageSize);
            paginationElem.MaxPageCount = pageCount;
            ProductsCountText.Content = $"Всего товаров: {_products.Count}";
            SelectProductTypes.ItemsSource = typeProducts.Select(p => p.name);
        }

        private async  void deleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem == null)
            {
                Growl.Error("Выберите админа!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var selectedProduct = ProductsList.SelectedItem as Products;
            ConnectionDb.db.Products.Remove(selectedProduct);
            ConnectionDb.db.SaveChanges();
            UpdateProductsList();
            Growl.Success("Удаление прошло успешно");
            await Task.Delay(1500);
            Growl.Clear();

        }
        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            ProductsList.ItemsSource = _products.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }
        private async void redactProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem == null)
            {
                Growl.Error("Выберите админа!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var redactProductDialog = new RedactProduct(this, ProductsList.SelectedItem as Products);
            redactProductDialog.Show();
        }

        private void addProduct_Click(object sender, RoutedEventArgs e)
        {
            var AddProductDialog = new AddProduct(this);
            AddProductDialog.Show();
        }

        private void ProductsSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = ProductsSearchText.Text.ToLower();
            var filtered = _products.Where(a => a.name.ToLower().Contains(searchText)
            || a.TypeProducts.name.ToLower().Contains(searchText)
            ).ToList();
            ProductsList.ItemsSource = filtered;
        }
        public async void UpdateProductsList()
        {
            var index = paginationElem.PageIndex;
            List<Products> newItems = await ConnectionDb.db.Products.ToListAsync();
            ProductsList.ItemsSource = newItems.Skip((paginationElem.PageIndex - 1) * pageSize).Take(pageSize).ToList();
            _products = newItems;
            ProductsCountText.Content = $"Всего товаров: {newItems.Count}";
            pageCount = (int)Math.Round(Convert.ToDouble(_products.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            paginationElem.PageIndex = index;
        }

        private void productTypes_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void SelectProductTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTypes = SelectProductTypes.SelectedItems;
            if (selectedTypes.Count == 0)
            {
                ProductsList.ItemsSource = _products;
                return;
            }
            var filtered = _products.Where(p => selectedTypes.Contains(p.TypeProducts.name)).ToList();
            ProductsList.ItemsSource = filtered;
        }

        private void updateProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateProductsList();
        }
    }
}
