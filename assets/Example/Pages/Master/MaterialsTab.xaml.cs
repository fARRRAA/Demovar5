using BeautySalonWpf.WindowDialogs.AdminPage.ProductDialog;
using BeautySalonWpf.WindowDialogs.AdminPage.Products;
using BeautySalonWpf.WindowDialogs.MasterPage;
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

namespace BeautySalonWpf.Pages.Master
{
    /// <summary>
    /// Логика взаимодействия для MaterialsTab.xaml
    /// </summary>
    public partial class MaterialsTab : Page
    {
        private List<ProductReceiveRequest> _products;
        private int pageCount;
        private int pageSize = 12;
        private List<TypeProducts> typeProducts = ConnectionDb.db.TypeProducts.ToList();
        private Masters _master;
        public MaterialsTab(Masters master)
        {
            InitializeComponent();
            _products = ConnectionDb.db.ProductReceiveRequest.Where(p=>p.masterId==master.masterId).ToList();
            pageCount = (int)Math.Round(Convert.ToDouble(_products.Count / pageSize)) + 1;
            ProductsList.ItemsSource = _products.Take(pageSize);
            paginationElem.MaxPageCount = pageCount;
            ProductsCountText.Content = $"Всего материалов: {_products.Count}";
            SelectProductTypes.ItemsSource = typeProducts.Select(p => p.name);
            _master = master;
        }

        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            ProductsList.ItemsSource = _products.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }

        private void addProduct_Click(object sender, RoutedEventArgs e)
        {
            var AddProductDialog = new AddMaterialReceive(this, _master);
            AddProductDialog.Show();
        }

        private void ProductsSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = ProductsSearchText.Text.ToLower();
            var filtered = _products.Where(a => a.Products.name.ToLower().Contains(searchText)
            || a.Products.TypeProducts.name.ToLower().Contains(searchText)
            ).ToList();
            ProductsList.ItemsSource = filtered;
        }
        public async void UpdateProductsList()
        {
            var index = paginationElem.PageIndex;
            List<ProductReceiveRequest> newItems = await ConnectionDb.db.ProductReceiveRequest.ToListAsync();
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
            var filtered = _products.Where(p => selectedTypes.Contains(p.Products.TypeProducts.name)).ToList();
            ProductsList.ItemsSource = filtered;
        }

        private void updateProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateProductsList();
        }
    }
}
