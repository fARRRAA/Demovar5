using HandyControl.Controls;
using HandyControl.Themes;
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
using System.Data.Entity;
using System.ComponentModel;
using BeautySalonWpf.WindowDialogs.AdminPage.SalaryDialogs;

namespace BeautySalonWpf.Pages.Admin.Tabs
{
    /// <summary>
    /// Логика взаимодействия для SalariesTab.xaml
    /// </summary>
    public partial class SalariesTab : Page
    {
        private List<MastersSalaries> _salaries;
        private List<Masters> _masters;
        private int? _selectedMonth;
        private int? _selectedYear;
        private int? _selectedMasterId;

        public SalariesTab()
        {
            InitializeComponent();
            InitializeFilters();
            LoadData();
        }

        private void InitializeFilters()
        {
            try
            {
                // Загрузка мастеров
                _masters = ConnectionDb.db.Masters.ToList();

                // Создаем список мастеров с полными именами
                var mastersWithFullName = _masters.Select(m => new
                {
                    MasterId = m.masterId,
                    FullName = $"{m.Lname} {m.Fname} {m.Patronymic}"
                }).ToList();

                // Добавляем опцию "Все мастера"
                var mastersList = new List<object>
                {
                    new { MasterId = (int?)null, FullName = "Все мастера" }
                };
                mastersList.AddRange(mastersWithFullName);

                MasterComboBox.ItemsSource = mastersList;
                MasterComboBox.SelectedIndex = 0;

                // Загрузка годов из базы данных
                var years = ConnectionDb.db.MastersSalaries
                    .Select(s => s.year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();

                // Если нет данных, добавляем текущий год
                if (years.Count == 0 || !years.Contains(DateTime.Now.Year))
                {
                    years.Add(DateTime.Now.Year);
                    years = years.OrderByDescending(y => y).ToList();
                }

                // Добавляем опцию "Все года"
                var yearsList = new List<object>
                {
                    new { Year = (int?)null, DisplayName = "Все года" }
                };
                yearsList.AddRange(years.Select(y => new { Year = (int?)y, DisplayName = y.ToString() }));

                YearComboBox.ItemsSource = yearsList;
                YearComboBox.DisplayMemberPath = "DisplayName";
                YearComboBox.SelectedValuePath = "Year";
                
                // Выбираем текущий год по умолчанию
                var currentYearItem = yearsList.FirstOrDefault(y => (y as dynamic).Year == DateTime.Now.Year);
                if (currentYearItem != null)
                    YearComboBox.SelectedItem = currentYearItem;
                else
                    YearComboBox.SelectedIndex = 0;

                // Устанавливаем текущий месяц
                int currentMonth = DateTime.Now.Month;
                MonthComboBox.SelectedIndex = currentMonth - 1; // -1 потому что индексы с 0
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка инициализации фильтров: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                // Получаем базовый запрос
                var query = ConnectionDb.db.MastersSalaries.AsQueryable();

                // Применяем фильтры
                if (_selectedMonth.HasValue)
                {
                    query = query.Where(s => s.month == _selectedMonth.Value);
                }

                if (_selectedYear.HasValue)
                {
                    query = query.Where(s => s.year == _selectedYear.Value);
                }

                if (_selectedMasterId.HasValue)
                {
                    query = query.Where(s => s.masterId == _selectedMasterId.Value);
                }

                // Загружаем данные с включением связанной информации о мастерах
                _salaries = query.Include(s => s.Masters).ToList();

                // Проверяем, есть ли данные за текущий месяц и год
                if (_selectedMonth.HasValue && _selectedYear.HasValue && 
                    !HasSalaryRecordsForCurrentMonthYear(_selectedMonth.Value, _selectedYear.Value))
                {
                    // Если нет, автоматически создаем записи для всех мастеров
                    CreateSalaryRecordsForAllMasters(_selectedMonth.Value, _selectedYear.Value);
                    
                    // Обновляем запрос после создания записей
                    query = ConnectionDb.db.MastersSalaries.AsQueryable();
                    if (_selectedMonth.HasValue)
                        query = query.Where(s => s.month == _selectedMonth.Value);
                    if (_selectedYear.HasValue)
                        query = query.Where(s => s.year == _selectedYear.Value);
                    if (_selectedMasterId.HasValue)
                        query = query.Where(s => s.masterId == _selectedMasterId.Value);
                    
                    _salaries = query.Include(s => s.Masters).ToList();
                }

                // Обновляем список
                SalariesList.ItemsSource = _salaries;

                // Обновляем счетчик
                UpdateSalaryCount();
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private bool HasSalaryRecordsForCurrentMonthYear(int month, int year)
        {
            return ConnectionDb.db.MastersSalaries.Any(s => s.month == month && s.year == year);
        }

        private void CreateSalaryRecordsForAllMasters(int month, int year)
        {
            try
            {
                // Получаем список всех мастеров
                var masterIds = ConnectionDb.db.Masters.Select(m => m.masterId).ToList();

                // Для каждого мастера создаем запись о зарплате с нулевой суммой
                foreach (var masterId in masterIds)
                {
                    // Проверяем, нет ли уже записи для этого мастера за указанный месяц и год
                    var existingRecord = ConnectionDb.db.MastersSalaries
                        .FirstOrDefault(s => s.masterId == masterId && s.month == month && s.year == year);

                    if (existingRecord == null)
                    {
                        // Создаем новую запись
                        var newSalary = new MastersSalaries
                        {
                            masterId = masterId,
                            year = year,
                            month = month,
                            salary = 0 // Начальная зарплата - 0
                        };

                        ConnectionDb.db.MastersSalaries.Add(newSalary);
                    }
                }

                // Сохраняем изменения
                ConnectionDb.db.SaveChanges();
                Growl.Info($"Созданы записи зарплат мастеров за {GetMonthName(month)} {year}.");
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка создания записей зарплат: {ex.Message}");
            }
        }

        private string GetMonthName(int month)
        {
            switch (month)
            {
                case 1: return "Январь";
                case 2: return "Февраль";
                case 3: return "Март";
                case 4: return "Апрель";
                case 5: return "Май";
                case 6: return "Июнь";
                case 7: return "Июль";
                case 8: return "Август";
                case 9: return "Сентябрь";
                case 10: return "Октябрь";
                case 11: return "Ноябрь";
                case 12: return "Декабрь";
                default: return "";
            }
        }

        private void UpdateSalaryCount()
        {
            if (_salaries != null)
            {
                SalariesCountText.Content = $"Всего записей: {_salaries.Count}";
            }
            else
            {
                SalariesCountText.Content = "Всего записей: 0";
            }
        }

        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _selectedMonth = selectedItem.Tag != null ? Convert.ToInt32(selectedItem.Tag) : (int?)null;
                LoadData();
            }
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearComboBox.SelectedValue != null)
            {
                _selectedYear = YearComboBox.SelectedValue as int?;
                LoadData();
            }
        }

        private void MasterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MasterComboBox.SelectedItem != null)
            {
                dynamic selectedMaster = MasterComboBox.SelectedItem;
                _selectedMasterId = selectedMaster.MasterId;
                LoadData();
            }
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            MasterComboBox.SelectedIndex = 0;
            YearComboBox.SelectedIndex = 0;
            MonthComboBox.SelectedIndex = DateTime.Now.Month - 1;
        }

        private void AddBonusButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка выбранной записи
                if (SalariesList.SelectedItem is MastersSalaries selectedSalary)
                {
                    // Создаем диалог для ввода премии
                    var dialog = new AddBonusDialog(selectedSalary);

                    dialog.ShowDialog();
                    if (dialog.DialogResult == true)
                    {
                        // Обновляем данные в интерфейсе
                        LoadData();
                        
                        // Выводим сообщение об успешном добавлении премии
                        Growl.Success($"Премия успешно добавлена мастеру: {selectedSalary.Masters?.Lname} {selectedSalary.Masters?.Fname}");
                    }
                }
                else
                {
                    Growl.Warning("Выберите запись для начисления премии");
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка при добавлении премии: {ex.Message}");
            }
        }
    }
}
