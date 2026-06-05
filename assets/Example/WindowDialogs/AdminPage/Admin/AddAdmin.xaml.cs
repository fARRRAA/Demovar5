using BeautySalonWpf.Pages.Admin.Tabs;
using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace BeautySalonWpf.WindowDialogs.Admin
{
    /// <summary>
    /// Логика взаимодействия для AddAdmin.xaml
    /// </summary>
    public partial class AddAdmin : System.Windows.Window
    {
        private string adminProfilePhotoFolder = "C:\\Users\\Ильдар\\source\\repos\\BeautySalonWpf\\BeautySalonWpf\\imgs\\pfp\\admins\\".Replace("\\", "/");
        private Admins tempAdmin=new Admins();
        private AdminTab _owner;
        public AddAdmin(AdminTab owner)
        {
            InitializeComponent();
            _owner = owner;
        }

        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ConfirmAddBtn_Click(object sender, RoutedEventArgs e)
        {
            var inputs = new[]
               {
        LoginText.Text,
        FNameText.Text,
        LNameText.Text,
       PatronymicText.Text,
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

            var admin = await ConnectionDb.db.Admins.FirstOrDefaultAsync(a => a.login == LoginText.Text);
            if (admin != null)
            {
                Growl.Error("Админ с таким логином существует.");
                await Task.Delay(1500);
                Growl.Clear();
                return;
            }

            tempAdmin.Fname = FNameText.Text;
            tempAdmin.Lname = LNameText.Text;
            tempAdmin.email = EmailText.Text;
            tempAdmin.Patronymic = PatronymicText.Text;
            tempAdmin.password = PasswordText.Password;
            tempAdmin.dateBirth = DateBirthText.SelectedDate;
            tempAdmin.phone = PhoneText.Text;
            tempAdmin.login = LoginText.Text;
            tempAdmin.roleId = 1;
            ConnectionDb.db.Admins.Add(tempAdmin);
            ConnectionDb.db.SaveChanges();
            Growl.Success("Добавление прошло успешно");
            await Task.Delay(1000);
            Growl.Clear();
            _owner.UpdateAdminsList();
            this.Close();
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
                    tempAdmin.photo = $"/imgs/pfp/admins/{fileName}";
                }
                catch (Exception ex)
                {
                    Growl.Error($"Ошибка при добавлении фотографии: {ex.Message}");
                    await Task.Delay(5500);
                    Growl.Clear();
                }
            }
            void createreport()
            {
                try
                {
                    string folderPath = @"C:\Users\Ильдар\source\repos\BeautySalonWpf\BeautySalonWpf\docs\";

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = System.IO.Path.Combine(folderPath, "example.txt");

                    string textToWrite = "Привет, это пример текста для записи в файл!";

                    File.WriteAllText(filePath, textToWrite);

                    Console.WriteLine($"Файл успешно создан и сохранён по пути: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
        }
    }
}
