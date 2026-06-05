using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
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
using System.Windows.Shapes;
using System.IO;
using BeautySalonWpf.Pages.Admin.Tabs;
namespace BeautySalonWpf.WindowDialogs
{
    /// <summary>
    /// Логика взаимодействия для RedactAdmin.xaml
    /// </summary>
    public partial class RedactAdmin : System.Windows.Window
    {
        private Admins _admin;
        private string adminProfilePhotoFolder = "C:\\Users\\Ильдар\\source\\repos\\BeautySalonWpf\\BeautySalonWpf\\imgs\\pfp\\admins\\".Replace("\\", "/");
        string removePath = "/imgs/pfp/admins/";
        private AdminTab _owner;
        public RedactAdmin(Admins admin,AdminTab owner)
        {
            InitializeComponent();
            _admin = admin;
            FillFieldsWInfo();
            _owner = owner;
        }

        private void CloseRedactBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async void ConfirmRedactBtn_Click(object sender, RoutedEventArgs e)
        {
            var inputs = new[]
               {
        LoginText.Text,
        FNameText.Text,
        LNameText.Text,
        PhoneText.Text,
        EmailText.Text,
        PasswordText.Password
                };

            if (inputs.Any(string.IsNullOrWhiteSpace) || !DateBirthText.SelectedDate.HasValue)
            {
                Growl.Error("Заполните все поля");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var check = await ConnectionDb.db.Admins.FirstOrDefaultAsync(a => a.login == LoginText.Text&&a.adminId!=_admin.adminId);
            if (check != null)
            {
                Growl.Error("Администратор с таким логином уже есть.");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }
            var admin = await ConnectionDb.db.Admins.FirstOrDefaultAsync(a => a.login == _admin.login);
            if (admin == null)
            {
                Growl.Error("Администратор не найден.");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }

            admin.Fname = FNameText.Text;
            admin.Lname = LNameText.Text;
            admin.email = EmailText.Text;
            admin.password = PasswordText.Password;
            admin.dateBirth = DateBirthText.SelectedDate;
            admin.phone = PhoneText.Text;
            admin.login = LoginText.Text;

            await ConnectionDb.db.SaveChangesAsync();

            Growl.Success("Редактирование прошло успешно");
            await Task.Delay(1000);
            Growl.Clear();
            _owner.UpdateAdminsList();
            this.Close();
        }
        private void FillFieldsWInfo()
        {
            FNameText.Text = _admin.Fname;
            LNameText.Text = _admin.Lname;
            PatronymicText.Text = _admin.Patronymic;
            DateBirthText.SelectedDate = _admin.dateBirth;
            PhoneText.Text = _admin.phone;
            EmailText.Text = _admin.email;
            LoginText.Text = _admin.login;
            if (!string.IsNullOrWhiteSpace(_admin.photo))
            {
                Photo.Source = new BitmapImage(new Uri(System.IO.Path.Combine(adminProfilePhotoFolder, _admin.photo.Replace(removePath,string.Empty)), UriKind.RelativeOrAbsolute));
            }
        }
        private async void addPhoto_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Выберите фотографию"
            };
            if (!Directory.Exists(adminProfilePhotoFolder))
            {
                Directory.CreateDirectory(adminProfilePhotoFolder);
            }
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                string destinationPath = System.IO.Path.Combine(adminProfilePhotoFolder, fileName);

                try
                {
                    File.Copy(openFileDialog.FileName, destinationPath, overwrite: true);
                    Photo.Source = new BitmapImage(new Uri(destinationPath, UriKind.RelativeOrAbsolute));
                    var admin = await ConnectionDb.db.Admins.FirstOrDefaultAsync(a => a.login == _admin.login);
                    admin.photo = $"/imgs/pfp/admins/{fileName}";
                    ConnectionDb.db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Growl.Error($"Ошибка при добавлении фотографии: {ex.Message}");
                    await Task.Delay(5500);
                    Growl.Clear();
                }
            }
        }

        //private async void deletePhoto_Click(object sender, RoutedEventArgs e)
        //{
        //    string destinationPath = System.IO.Path.Combine(adminProfilePhotoFolder, _admin.photo.Replace(removePath,string.Empty));
        //    if (File.Exists(destinationPath))
        //    {
        //        try
        //        {
        //            File.Delete(destinationPath);
        //            Photo.Source = null;
        //            var admin = await ConnectionDb.db.Admins.FirstOrDefaultAsync(a => a.login == _admin.login);
        //            _admin.photo = ""; 
        //            await ConnectionDb.db.SaveChangesAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            Growl.Error($"Ошибка при удалении фотографии: {ex.Message}");
        //        }
        //    }
        //    else
        //    {
        //        Growl.Error("Фотография не найдена.");
        //    }

        //}
    }
    //catch (DbEntityValidationException dbEx)
    //{
    //    foreach (var validationErrors in dbEx.EntityValidationErrors)
    //    {
    //        foreach (var validationError in validationErrors.ValidationErrors)
    //        {
    //            Growl.Error($"Свойство: {validationError.PropertyName}, Ошибка: {validationError.ErrorMessage}");
    //        }
    //    }
    //}
    //catch (Exception ex)
    //{
    //    Growl.Error($"Произошла ошибка: {ex.Message}");
    //}

}
//}
