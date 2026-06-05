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

namespace BeautySalonWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : Page
    {

        private MainWindow _mw;
        public SignUp(MainWindow mw)
        {
            InitializeComponent();
            _mw = mw;
            _mw.ChangeWindowSize(800, 700);
        }

        private void AlreadyRegistered_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(new SignIn(_mw));
        }
        private async void RegisterBtnUser_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextUser.Text;
            var FName = FNameTextUser.Text;
            var LName = LNameTextUser.Text;
            var dateBirth = DateBirthText.SelectedDate;
            var phone = PhoneTextUser.Text;
            var email = EmailTextUser.Text;
            var password = PasswordTextUser.Password;
            var temp = new Clients()
            {
                login = login,
                password = password,
                FName = FName,
                Lname = LName,
                dateBirth = dateBirth,
                phone = phone,
                email = email
            };
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(FName) || string.IsNullOrWhiteSpace(LName)||string.IsNullOrWhiteSpace(dateBirth.ToString()) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return; 
            }
            var check = await ConnectionDb.db.Clients.FirstOrDefaultAsync(u => u.login == temp.login);
            if (check != null)
            {
                Growl.Error("Аккаунт с таким логином уже есть");
                return;
            }
             ConnectionDb.db.Clients.Add(temp);
             ConnectionDb.db.SaveChanges();
            Growl.Success("вы успешно зарегистрировались");
            await Task.Delay(1500);
            Growl.Clear();
            _mw.MainFrame.NavigationService.Navigate(new SignIn(_mw));
        }
    }
}
