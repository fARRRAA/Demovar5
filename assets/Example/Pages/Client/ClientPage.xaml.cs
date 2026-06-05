using BeautySalonWpf.Pages.Client.Tabs;
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

namespace BeautySalonWpf.Pages.Client
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private MainWindow _mw;
        private Clients _client;
        public ClientPage(MainWindow mw,Clients client)
        {
            InitializeComponent();
            _mw = mw;
            AppointmentsFrame.Navigate(new ClientAppointments(client));
            _mw.ChangeWindowSize(900, 1400);
        }

        private async void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(250);
            _mw.MainFrame.Navigate(new SignIn(_mw));
        }
    }
}
