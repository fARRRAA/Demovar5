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
    /// Логика взаимодействия для ProviderTab.xaml
    /// </summary>
    public partial class ProviderTab : Page
    {
        private List<Provider> _providers;
        private int pageCount;
        private int pageSize = 12;
        public ProviderTab()
        {

            InitializeComponent();
            _providers = ConnectionDb.db.Provider.ToList();
            ProvidersList.ItemsSource = _providers.Take(pageSize);
            pageCount = (int)Math.Round(Convert.ToDouble(_providers.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            ProvidersCountText.Content = $"Всего поставщиков: {_providers.Count}";
        }

        private async void deleteProvider_Click(object sender, RoutedEventArgs e)
        {
            if (ProvidersList.SelectedItem is Provider selectedProvider)
            {

                ConnectionDb.db.Provider.Remove(selectedProvider);
                ConnectionDb.db.SaveChanges();
                Growl.Success("Поставщик удалён");
                await Task.Delay(1500);
                UpdateProvidersList();
            }
            else
            {
                Growl.Error(message: "выберите поставку");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
        }

        
        private void ProviderSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = ProviderSearchText.Text.ToLower();
            var filtered = _providers.Where(a => a.name.ToLower().Contains(searchText)
            || a.name.ToLower().Contains(searchText)
            || a.INN.ToLower().Contains(searchText)
            || a.phone.ToLower().Contains(searchText)
            ).ToList();
            ProvidersList.ItemsSource = filtered;
        }
        private void redactProvider_Click(object sender, RoutedEventArgs e)
        {
            if (ProvidersList.SelectedItem is Provider selectedProvider)
            {
                var redactWindow = new BeautySalonWpf.WindowDialogs.Provider.RedactProvider(this, selectedProvider);
                redactWindow.ShowDialog();
            }
            else
            {
                HandyControl.Controls.Growl.Info("Выберите поставщика для редактирования");
            }
        }
        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            ProvidersList.ItemsSource = _providers.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }
        private void addProvider_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new BeautySalonWpf.WindowDialogs.Provider.AddProvider(this);
            addWindow.ShowDialog();
        }

        private void updateProducts_Click()
        {

        }

        public async void UpdateProvidersList()
        {
            var index = paginationElem.PageIndex;
            var newItems = ConnectionDb.db.Provider.ToList();
            ProvidersList.ItemsSource = newItems.Skip((paginationElem.PageIndex - 1) * pageSize).Take(pageSize).ToList();
            _providers = newItems;
            pageCount = (int)Math.Round(Convert.ToDouble(_providers.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            paginationElem.PageIndex = index;
            ProvidersCountText.Content = $"Всего поставщиков: {_providers.Count}";
        }

        private void updateProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateProvidersList();
        }
    }
}
