using BeautySalonWpf.WindowDialogs.Master;
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
    /// Логика взаимодействия для MastersTab.xaml
    /// </summary>
    public partial class MastersTab : Page
    {
        private Admins _admin;
        private List<Masters> _masters;
        private int pageCount;
        private int pageSize = 12;
        public MastersTab(Admins admin)
        {
            InitializeComponent();
            _admin = admin;
            _masters = ConnectionDb.db.Masters.ToList();
            MastersList.ItemsSource = _masters.Take(pageSize);
            pageCount = (int)Math.Round(Convert.ToDouble(_masters.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            MastersCountText.Content = $"Всего мастеров: {_masters.Count}";
        }

        private void MastersSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = MastersSearchText.Text.ToLower();
            var filtered = _masters.Where(a => a.Lname.ToLower().Contains(searchText)
            || a.Fname.ToLower().Contains(searchText)
            || a.MastersQualifications.TypeServices.name.ToLower().Contains(searchText)
            || a.login.ToLower().Contains(searchText)
            || a.email.ToLower().Contains(searchText)
            || a.MastersSkills.name.ToLower().Contains(searchText)
            ).ToList();
            MastersList.ItemsSource = filtered;
        }

        private async void deleteMaster_Click(object sender, RoutedEventArgs e)
        {
            if (MastersList.SelectedItem == null)
            {
                Growl.Error("Выберите админа!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var selectedMasters = MastersList.SelectedItem as Masters;
            if (selectedMasters == null)
            {
                Growl.Error("Выберите админа!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            ConnectionDb.db.Masters.Remove(selectedMasters);
            ConnectionDb.db.SaveChanges();
            UpdateMastersList();
            Growl.Success("Удаление прошло успешно");
            await Task.Delay(1500);
            Growl.Clear();
        }

        private async void redactMaster_Click(object sender, RoutedEventArgs e)
        {
            if (MastersList.SelectedItem == null)
            {
                Growl.Error("Выберите админа!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var selectedMaster = MastersList.SelectedItem as Masters;
            var RedactMasterDialog = new RedactMaster(selectedMaster,this);
            RedactMasterDialog.Show();
        }

        private void addMaster_Click(object sender, RoutedEventArgs e)
        {
            var AddMasterDialog = new AddMaster(this);
            AddMasterDialog.ShowDialog();
        }
        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            MastersList.ItemsSource = _masters.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }
        public async void UpdateMastersList()
        {
            var index = paginationElem.PageIndex;
            var newItems = await ConnectionDb.db.Masters.ToListAsync();
            MastersList.ItemsSource = newItems.Skip((paginationElem.PageIndex - 1) * pageSize).Take(pageSize).ToList();
            _masters = newItems;
            MastersCountText.Content = $"Всего мастеров: {newItems.Count}";
            pageCount = (int)Math.Round(Convert.ToDouble(_masters.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            paginationElem.PageIndex = index;

        }

        private void updateProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateMastersList();
        }
    }
}
