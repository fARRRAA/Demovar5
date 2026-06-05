using BeautySalonWpf.WindowDialogs.AdminPage.Appointmentdialogs;
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
    /// Логика взаимодействия для AppointmentsTab.xaml
    /// </summary>
    public partial class AppointmentsTab : Page
    {
        private List<Appointments> _appointments;
        private List<Appointments> _allAppointments;

        private int pageCount;
        private int pageSize = 9;
        public AppointmentsTab()
        {
            InitializeComponent();
            _appointments = ConnectionDb.db.Appointments.OrderByDescending(a=>a.date).ToList();
            AppointmentsList.ItemsSource = _appointments.Take(pageSize);
            _allAppointments = _appointments;
            pageCount = (int)Math.Round(Convert.ToDouble(_appointments.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            AppointmentsCountText.Content = $"Всего записей: {_appointments.Count}";

        }

        private async void deleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            var app = AppointmentsList.SelectedItem as Appointments;
            var appItems = ConnectionDb.db.AppointmentsServices.Where(x=>x.appointmentId==app.id).ToList();
            if (app != null)
            {
                ConnectionDb.db.Appointments.Remove(app);
                if(appItems != null)
                {
                    ConnectionDb.db.AppointmentsServices.RemoveRange(appItems);
                    ConnectionDb.db.SaveChanges();
                    Growl.Success("Удаление прошло успешно!");
                    await Task.Delay(1500);
                    Growl.Clear();
                    UpdateAppoinmentsList();
                }
            }
        }

        private void addAppointment_Click(object sender, RoutedEventArgs e)
        {
            var addAppointmentDialog = new AddAppointmentDialog(this);
            addAppointmentDialog.Show();
        }
        private void AppointmentSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = AppointmentSearchText.Text.ToLower();
            var filtered = _appointments.Where(a => a.Masters.Fname.ToLower().Contains(searchText)
            || a.Masters.Lname.ToLower().Contains(searchText)
           || a.Clients.FName.ToLower().Contains(searchText)
           || a.Clients.Lname.ToLower().Contains(searchText)
           ||a.AppointmentStatus.name.ToLower().Contains(searchText)
            ).ToList();
            AppointmentsList.ItemsSource = filtered;
            //filtered[0].;
        }
        public async void UpdateAppoinmentsList()
        {
            var index = paginationElem.PageIndex;
            var newItems = await ConnectionDb.db.Appointments.OrderByDescending(a => a.date).ToListAsync();
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
            var filtered = _appointments.Where(a => a.date ==Date).ToList();
            AppointmentsList.ItemsSource = filtered;
        }

        private async void redactAppointment_Click(object sender, RoutedEventArgs e)
        {
            var selectedAppointment = AppointmentsList.SelectedItem as Appointments;
            if(selectedAppointment == null)
            {
                Growl.Error("Выберите запись!");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var appointment = await ConnectionDb.db.Appointments.FirstOrDefaultAsync(a=>a.id==selectedAppointment.id);
            appointment.statusId = 1;
            var salary = await ConnectionDb.db.MastersSalaries.FirstOrDefaultAsync(s => s.masterId == selectedAppointment.masterId && s.month == DateTime.Now.Month && s.year == DateTime.Now.Year);
            var mastersGain = appointment.totalSum * ((double)selectedAppointment.Masters.MastersSkills.rate / 100);
            salary.salary += (int)mastersGain;
            await ConnectionDb.db.SaveChangesAsync();
            Growl.Success("Изменение прошло успешно!");
            await Task.Delay(1500);
            Growl.Clear();
            UpdateAppoinmentsList();
        }

        private void CancelFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            AppointmentsList.ItemsSource = _allAppointments;
            AppointmentSearchText.Text = string.Empty;
            DateText.SelectedDate = null;
        }

        private void IsCompletedBtn_Checked(object sender, RoutedEventArgs e)
        {
            var filtered = _appointments.Where(a=>a.statusId==1).ToList();
            AppointmentsList.ItemsSource = filtered;
        }

        private void IsCompletedBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.IsChecked == true)
            {
                // Отменяем событие (т.е. не даем пользователю переключить RadioButton).
                e.Handled = true;
            }
        }

        private void IsCreatedBtn_Checked(object sender, RoutedEventArgs e)
        {
            var filtered = _appointments.Where(a => a.statusId == 2).ToList();
            AppointmentsList.ItemsSource = filtered;
        }

        private void ISCancelledBtn_Checked(object sender, RoutedEventArgs e)
        {
            var filtered = _appointments.Where(a => a.statusId == 3).ToList();
            AppointmentsList.ItemsSource = filtered;
        }

        private void updateProducts_Click(object sender, RoutedEventArgs e)
        {
            UpdateAppoinmentsList();
        }
    }
}
