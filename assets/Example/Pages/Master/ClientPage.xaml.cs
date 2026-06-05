using BeautySalonWpf.WindowDialogs.Client;
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
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private int pageCount;
        private int pageSize = 12;
        private List<Clients> _clients;
        public ClientPage()
        {
            InitializeComponent();
            var clients = ConnectionDb.db.Clients.ToList();
            ClientsList.ItemsSource = clients.Take(pageSize);
            _clients = clients;
            pageCount = (int)Math.Round(Convert.ToDouble(_clients.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            ClientsCountText.Content = $"Всего клиентов: {_clients.Count}";

        }
        private void ClientsSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = ClientsSearchText.Text.ToLower();
            var filtered = _clients.Where(a => a.Lname.ToLower().Contains(searchText)
            || a.FName.ToLower().Contains(searchText)
            || a.login.ToLower().Contains(searchText)
            || a.email.ToLower().Contains(searchText)
            ).ToList();
            ClientsList.ItemsSource = filtered;
        }
        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            ClientsList.ItemsSource = _clients.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }
        public async void UpdateClientsList()
        {

            var index = paginationElem.PageIndex;
            var newItems = await ConnectionDb.db.Clients.ToListAsync();
            ClientsList.ItemsSource = newItems.Skip((paginationElem.PageIndex - 1) * pageSize).Take(pageSize).ToList();
            _clients = newItems;
            ClientsCountText.Content = $"Всего клиентов: {newItems.Count}";
            pageCount = (int)Math.Round(Convert.ToDouble(_clients.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            paginationElem.PageIndex = index;
        }
        private async void redactClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsList.SelectedItem == null)
            {
                Growl.Error("Выберите клиента!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var selectedClient = ClientsList.SelectedItem as Clients;
            var clientRedactDialog = new MasterRedactClient(this, selectedClient);
            clientRedactDialog.Show();
        }
    }
}
