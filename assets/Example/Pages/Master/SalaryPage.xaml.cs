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

namespace BeautySalonWpf.Pages.Master
{

    public partial class SalaryPage : Page
    {
        private List<Appointments> _appointments;
        private List<Appointments> _allAppointments;
        private int pageCount;
        private int pageSize = 9;
        private Masters _master;
        public SalaryPage(Masters master)      
        {
            InitializeComponent();
            _appointments = ConnectionDb.db.Appointments.Where(x => x.masterId == master.masterId&&x.statusId==1).OrderByDescending(a => a.date).ToList();
            AppointmentsList.ItemsSource = _appointments.Take(pageSize);
            _allAppointments = _appointments;
            pageCount = (int)Math.Round(Convert.ToDouble(_appointments.Count / pageSize)) + 1;
            paginationElem.MaxPageCount = pageCount;
            _master = master;
            AllTImeRadioBtn.IsChecked = true;
            var all = _appointments.Where(x => x.date.Value.Day == DateTime.Now.Day  && x.date.Value.Month == DateTime.Now.Month && x.date.Value.Year == DateTime.Now.Year).ToList();
            int? timeWorked = all.Sum(x => x.totalDuration);
            int totalMinutes = timeWorked.Value; 
            int hours = totalMinutes / 60;  
            int minutes = totalMinutes % 60;
            TodayTimeWorked.Text = $"{hours} ч. {minutes} мин.";

            double? salaryEarned = all.Sum(x => x.totalSum*((double)x.Masters.MastersSkills.rate / 100));
            TodaySalaryEarned.Text = $"{salaryEarned} ₽";
        }
        public async void UpdateAppoinmentsList()
        {
            var index = paginationElem.PageIndex;
            var newItems = ConnectionDb.db.Appointments.Where(x => x.masterId == _master.masterId).OrderByDescending(a => a.date).ToList();
            AppointmentsList.ItemsSource = newItems.Skip((paginationElem.PageIndex - 1) * pageSize).Take(pageSize).ToList();
            _appointments = newItems;
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
        private void CancelFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            AppointmentsList.ItemsSource = _allAppointments;
            AppointmentSearchText.Text = string.Empty;
            DateText.SelectedDate = null;
            AllTImeRadioBtn.IsChecked = true;

        }

        private void IsCompletedBtn_Checked(object sender, RoutedEventArgs e)
        {

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

        }

        private void AllTImeRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            var all = _appointments;
            AppointmentsList.ItemsSource = all;
        }

        private void TodayRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            var all = _appointments.Where(x=>x.date.Value.Day == DateTime.Now.Day
            && x.date.Value.Month == DateTime.Now.Month
            && x.date.Value.Year == DateTime.Now.Year
            ).ToList();
            AppointmentsList.ItemsSource = all;

        }

        private void ThisMonthRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            var all = _appointments.Where(x => x.date.Value.Month == DateTime.Now.Month && x.date.Value.Year == DateTime.Now.Year).ToList();
            AppointmentsList.ItemsSource = all;
        }
    }
}
