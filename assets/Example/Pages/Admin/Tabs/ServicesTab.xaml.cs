using BeautySalonWpf.WindowDialogs.AdminPage.Service;
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
    /// Логика взаимодействия для ServicesTab.xaml
    /// </summary>
    public partial class ServicesTab : Page
    {
        private List<Services> _services;
        private int pageCount;
        private int pageSize = 12;
        public ServicesTab()
        {
            InitializeComponent();
            _services = ConnectionDb.db.Services.ToList();
            ServicesList.ItemsSource = _services.Take(pageSize);
            pageCount = (int)Math.Round(Convert.ToDouble(_services.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            ServiceCountText.Content = $"Всего услуг: {_services.Count}";
        }
       
        private async void deleteService_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesList.SelectedItem == null)
            {
                Growl.Error("Выберите услугу!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var selectedService = ServicesList.SelectedItem as Services;
            ConnectionDb.db.Services.Remove(selectedService);
            ConnectionDb.db.SaveChanges();
            UpdateServicesList();
            Growl.Success("Удаление прошло успешно");
            await Task.Delay(1500);
            Growl.Clear();

        }

        private async void redactService_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesList.SelectedItem == null)
            {
                Growl.Error("Выберите услугу!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var selectedService = ServicesList.SelectedItem as Services;
            var redactServiceDialog = new RedactService(this,selectedService); 
            redactServiceDialog.Show();
        }

        private void addService_Click(object sender, RoutedEventArgs e)
        {
            var addServiceDialog = new AddService(this);
            addServiceDialog.Show();
        }

        private void ServiceSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = ServiceSearchText.Text.ToLower();
            var filtered = _services.Where(a => a.serviceName.ToLower().Contains(searchText)
            ).ToList();
            ServicesList.ItemsSource = filtered;
        }
        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            ServicesList.ItemsSource = _services.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }
        public async void UpdateServicesList()
        {
            var newItems = await ConnectionDb.db.Services.ToListAsync();
            ServicesList.ItemsSource = newItems.Skip((paginationElem.PageIndex - 1) * pageSize).Take(pageSize).ToList();
            _services = newItems;
            ServiceCountText.Content = $"Всего услуг: {newItems.Count}";
            pageCount = (int)Math.Round(Convert.ToDouble(_services.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            paginationElem.PageIndex = pageCount;
        }

        private void updateProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateServicesList();
        }
    }
}
