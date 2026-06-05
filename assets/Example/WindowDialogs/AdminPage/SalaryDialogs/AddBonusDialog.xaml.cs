using HandyControl.Controls;
using System;
using System.Windows;

namespace BeautySalonWpf.WindowDialogs.AdminPage.SalaryDialogs
{
    /// <summary>
    /// Логика взаимодействия для AddBonusDialog.xaml
    /// </summary>
    public partial class AddBonusDialog : System.Windows.Window
    {
        private MastersSalaries _salary;

        public AddBonusDialog(MastersSalaries salary)
        {
            InitializeComponent();
            _salary = salary;
            
            // Устанавливаем заголовок с информацией о мастере
            TitleText.Text = $"Добавление премии мастеру:\n{salary.Masters?.Lname} {salary.Masters?.Fname} {salary.Masters?.Patronymic}";
            
            // Устанавливаем фокус на поле ввода
            BonusAmountInput.Focus();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем значение премии из элемента ввода
                decimal bonusValue = (decimal)BonusAmountInput.Value;
                
                // Проверяем, что значение положительное
                if (bonusValue <= 0)
                {
                    Growl.Warning("Введите сумму премии больше нуля.");
                    return;
                }
                
                // Преобразуем decimal в int
                int bonusAmount = (int)bonusValue;
                
                // Обновляем запись в базе данных
                var salary = ConnectionDb.db.MastersSalaries.Find(_salary.id);
                if (salary != null)
                {
                    // Прибавляем премию к текущей зарплате
                    salary.salary = (salary.salary ?? 0) + bonusAmount;
                    ConnectionDb.db.SaveChanges();
                    
                    // Обновляем локальную копию
                    _salary.salary = salary.salary;
                    
                    DialogResult = true;
                    Close();
                }
                else
                {
                    Growl.Error("Запись не найдена в базе данных");
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка при добавлении премии: {ex.Message}");
            }
        }
    }
} 