using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using HandyControl.Controls;
using Microsoft.Win32;

namespace Demovar5.WindowDialogs
{
    /// <summary>
    /// Универсальное окно для добавления и редактирования товара
    /// </summary>
    public partial class AddEditProductWindow : Window, INotifyPropertyChanged
    {
        private Products _product;
        private bool _isEditMode;
        private string _selectedPhotoPath;
        private string _photoPreviewPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public Products Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(WindowTitle));
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }

        public string WindowTitle => IsEditMode ? "Редактирование товара" : "Добавление товара";
        public string SaveButtonText => IsEditMode ? "Сохранить" : "Добавить";

        public string PhotoPreviewPath
        {
            get => _photoPreviewPath;
            set
            {
                _photoPreviewPath = value;
                OnPropertyChanged(nameof(PhotoPreviewPath));
            }
        }

        // Конструктор для добавления нового товара
        public AddEditProductWindow()
        {
            InitializeComponent();
            IsEditMode = false;
            InitializeNewProduct();
            LoadComboBoxes();
            DataContext = this;
        }

        // Конструктор для редактирования существующего товара
        public AddEditProductWindow(Products product)
        {
            InitializeComponent();
            IsEditMode = true;
            Product = product;
            LoadComboBoxes();
            LoadProductData();
            DataContext = this;
        }

        private void InitializeNewProduct()
        {
            Product = new Products
            {
                ArticleNumber = "",
                ProductName = "",
                Price = 0,
                Discount = 0,
                QuantityInStock = 0,
                Description = "",
                PhotoPath = null
            };

            PhotoPreviewPath = "/Resources/Images/picture.png";
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var context = new demovar5Entities())
                {
                    // Загружаем категории
                    CategoryComboBox.ItemsSource = context.Categories.ToList();

                    // Загружаем производителей
                    ManufacturerComboBox.ItemsSource = context.Manufacturers.ToList();

                    // Загружаем поставщиков
                    SupplierComboBox.ItemsSource = context.Suppliers.ToList();

                    // Загружаем единицы измерения
                    UnitComboBox.ItemsSource = context.Units.ToList();
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки справочников: {ex.Message}");
            }
        }

        private void LoadProductData()
        {
            if (Product.PhotoPath != null)
            {
                PhotoPreviewPath = $"/Resources/Images/{Product.PhotoPath}";
            }
            else
            {
                PhotoPreviewPath = "/Resources/Images/picture.png";
            }
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фото товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Проверяем размер изображения
                    var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    
                    if (bitmap.PixelWidth > 300 || bitmap.PixelHeight > 200)
                    {
                        var result = System.Windows.MessageBox.Show(
                            $"Размер изображения ({bitmap.PixelWidth}x{bitmap.PixelHeight}) превышает рекомендуемый (300x200).\n\n" +
                            "Рекомендуется использовать изображение 300x200 пикселей для оптимального отображения.\n\n" +
                            "Продолжить?",
                            "Предупреждение",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.No)
                            return;
                    }

                    _selectedPhotoPath = openFileDialog.FileName;
                    PhotoPreviewPath = openFileDialog.FileName;
                    Growl.Success("Фото выбрано. Оно будет сохранено после нажатия кнопки 'Сохранить'/'Добавить'");
                }
                catch (Exception ex)
                {
                    Growl.Error($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }

        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            _selectedPhotoPath = null;
            Product.PhotoPath = null;
            PhotoPreviewPath = "/Resources/Images/picture.png";
            Growl.Info("Фото удалено");
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (!ValidateInput())
                return;

            try
            {
                using (var context = new demovar5Entities())
                {
                    if (IsEditMode)
                    {
                        // Редактирование существующего товара
                        var existingProduct = context.Products.Find(Product.ProductID);
                        if (existingProduct == null)
                        {
                            Growl.Error("Товар не найден");
                            return;
                        }

                        // Обновляем данные
                        existingProduct.ArticleNumber = Product.ArticleNumber;
                        existingProduct.ProductName = Product.ProductName;
                        existingProduct.CategoryID = Product.CategoryID;
                        existingProduct.ManufacturerID = Product.ManufacturerID;
                        existingProduct.SupplierID = Product.SupplierID;
                        existingProduct.UnitID = Product.UnitID;
                        existingProduct.Price = Product.Price;
                        existingProduct.Discount = Product.Discount;
                        existingProduct.QuantityInStock = Product.QuantityInStock;
                        existingProduct.Description = Product.Description;

                        // Обработка фото
                        if (_selectedPhotoPath != null)
                        {
                            // Удаляем старое фото
                            if (!string.IsNullOrEmpty(existingProduct.PhotoPath))
                            {
                                DeleteOldPhoto(existingProduct.PhotoPath);
                            }

                            // Сохраняем новое фото
                            existingProduct.PhotoPath = SavePhoto(_selectedPhotoPath);
                        }
                        else if (Product.PhotoPath == null && existingProduct.PhotoPath != null)
                        {
                            // Если фото было удалено
                            DeleteOldPhoto(existingProduct.PhotoPath);
                            existingProduct.PhotoPath = null;
                        }

                        context.SaveChanges();
                        Growl.Success("Товар успешно обновлен");
                    }
                    else
                    {
                        // Добавление нового товара
                        // Проверяем уникальность артикула
                        if (context.Products.Any(p => p.ArticleNumber == Product.ArticleNumber))
                        {
                            Growl.Warning("Товар с таким артикулом уже существует");
                            return;
                        }

                        // Сохраняем фото
                        if (_selectedPhotoPath != null)
                        {
                            Product.PhotoPath = SavePhoto(_selectedPhotoPath);
                        }

                        context.Products.Add(Product);
                        context.SaveChanges();
                        Growl.Success("Товар успешно добавлен");
                    }

                    await System.Threading.Tasks.Task.Delay(1000);
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(Product.ArticleNumber))
            {
                Growl.Warning("Введите артикул товара");
                ArticleNumberTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Product.ProductName))
            {
                Growl.Warning("Введите наименование товара");
                ProductNameTextBox.Focus();
                return false;
            }

            if (Product.CategoryID == 0)
            {
                Growl.Warning("Выберите категорию товара");
                CategoryComboBox.Focus();
                return false;
            }

            if (Product.ManufacturerID == 0)
            {
                Growl.Warning("Выберите производителя");
                ManufacturerComboBox.Focus();
                return false;
            }

            if (Product.SupplierID == 0)
            {
                Growl.Warning("Выберите поставщика");
                SupplierComboBox.Focus();
                return false;
            }

            if (Product.UnitID == 0)
            {
                Growl.Warning("Выберите единицу измерения");
                UnitComboBox.Focus();
                return false;
            }

            if (Product.Price < 0)
            {
                Growl.Warning("Цена не может быть отрицательной");
                PriceNumeric.Focus();
                return false;
            }

            if (Product.Discount < 0 || Product.Discount > 100)
            {
                Growl.Warning("Скидка должна быть от 0 до 100%");
                DiscountNumeric.Focus();
                return false;
            }

            if (Product.QuantityInStock < 0)
            {
                Growl.Warning("Количество на складе не может быть отрицательным");
                QuantityNumeric.Focus();
                return false;
            }

            return true;
        }

        private string SavePhoto(string sourcePath)
        {
            try
            {
                // Генерируем уникальное имя файла
                var extension = Path.GetExtension(sourcePath);
                var fileName = $"{Guid.NewGuid()}{extension}";
                
                // Путь к папке Resources/Images в проекте
                var projectPath = AppDomain.CurrentDomain.BaseDirectory;
                var targetFolder = Path.Combine(projectPath, "Resources", "Images");
                
                // Создаем папку если не существует
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var targetPath = Path.Combine(targetFolder, fileName);

                // Копируем файл
                File.Copy(sourcePath, targetPath, true);

                return fileName;
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка при сохранении фото: {ex.Message}");
                return null;
            }
        }

        private void DeleteOldPhoto(string photoPath)
        {
            try
            {
                if (string.IsNullOrEmpty(photoPath))
                    return;

                var projectPath = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(projectPath, "Resources", "Images", photoPath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // Не критичная ошибка, можно просто залогировать
                System.Diagnostics.Debug.WriteLine($"Ошибка при удалении старого фото: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
