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

namespace BeautySalonWpf.Pages.Client.Tabs
{
    /// <summary>
    /// Логика взаимодействия для ClientAppointments.xaml
    /// </summary>
    public partial class ClientAppointments : Page
    {
        private List<Appointments> _appointments;
        private List<Appointments> _allAppointments;
        private Clients _client;
        private int pageCount;
        private int pageSize = 9;
        public ClientAppointments(Clients client)
        {

            InitializeComponent();
            _appointments = ConnectionDb.db.Appointments.Where(x => x.clientId == client.userID).OrderByDescending(a => a.date).ToList();
            AppointmentsList.ItemsSource = _appointments.Take(pageSize);
            _allAppointments = _appointments;
            pageCount = (int)Math.Round(Convert.ToDouble(_appointments.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            AppointmentsCountText.Content = $"Всего записей: {_appointments.Count}";
            _client = client;
        }
        private void AppointmentSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = AppointmentSearchText.Text.ToLower();
            var filtered = _appointments.Where(a => a.Masters.Fname.ToLower().Contains(searchText)
            || a.Masters.Lname.ToLower().Contains(searchText)
           || a.Clients.FName.ToLower().Contains(searchText)
           || a.Clients.Lname.ToLower().Contains(searchText)
           || a.AppointmentStatus.name.ToLower().Contains(searchText)
            ).ToList();
            AppointmentsList.ItemsSource = filtered;
            //filtered[0].;
        }
        public async void UpdateAppoinmentsList()
        {
            var index = paginationElem.PageIndex;
            var newItems = ConnectionDb.db.Appointments.Where(x => x.clientId == _client.userID).OrderByDescending(a => a.date).ToList();
            AppointmentsList.ItemsSource = newItems.Skip((paginationElem.PageIndex - 1) * pageSize).Take(pageSize).ToList();
            _appointments = newItems;
            AppointmentsCountText.Content = $"Всего записей: {newItems.Count}";
            pageCount = (int)Math.Round(Convert.ToDouble(_appointments.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            paginationElem.PageIndex = index;
        }
        private void page_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            AppointmentsList.ItemsSource = _appointments.Skip((e.Info - 1) * pageSize).Take(pageSize).ToList();
        }
        private void DateText_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateText.SelectedDate == null)
            {
                AppointmentsList.ItemsSource = _allAppointments;
                return;
            }
            var Date = DateText.SelectedDate;
            var filtered = _appointments.Where(a => a.date == Date).ToList();
            AppointmentsList.ItemsSource = filtered;
        }

        private void redactAppointment_Click(object sender, RoutedEventArgs e)
        {
            //var add = new RegisterAppointment(this,_client);
            //add.Show();
        }
    }
}
