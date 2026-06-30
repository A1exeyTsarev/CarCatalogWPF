using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CarCatalogWPF.AppData;

namespace CarCatalogWPF.Pages
{
    public partial class AddEditCarPage : Page
    {
        private int _carId;
        private string _currentImageName = "";

        public AddEditCarPage(int carId)
        {
            InitializeComponent();
            _carId = carId;
            LoadComboBoxes();

            if (carId > 0)
            {
                LoadCarData(carId);
            }
        }

        private void LoadComboBoxes()
        {
            CbBrand.ItemsSource = AppConnect.model01.Brands.ToList();
            CbCountry.ItemsSource = AppConnect.model01.Countries.ToList();
        }

        private void LoadCarData(int id)
        {
            var car = AppConnect.model01.Cars.Find(id);
            if (car != null)
            {
                CbBrand.SelectedValue = car.IdBrand;
                CbCountry.SelectedValue = car.IdCountry;
                TbYear.Text = car.Year.ToString();
                TbPrice.Text = car.Price.ToString();

                if (!string.IsNullOrEmpty(car.ImageName))
                {
                    _currentImageName = car.ImageName;
                    string imagePath = GetImagePath(car.ImageName);
                    if (File.Exists(imagePath))
                    {
                        PhotoImage.Source = new BitmapImage(new Uri(imagePath));
                        TbImageName.Text = car.ImageName;
                    }
                }
            }
        }

        private string GetImagePath(string fileName)
        {
            return System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..\\..\\Images\\",
                fileName);
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            dialog.Title = "Выберите изображение";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = System.IO.Path.GetFileName(dialog.FileName);
                    string destinationPath = GetImagePath(fileName);

                    if (!File.Exists(destinationPath))
                    {
                        File.Copy(dialog.FileName, destinationPath, true);
                    }

                    PhotoImage.Source = new BitmapImage(new Uri(destinationPath));
                    TbImageName.Text = fileName;
                    _currentImageName = fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CbBrand.SelectedValue == null)
            {
                TbError.Text = "Выберите марку!";
                TbError.Visibility = Visibility.Visible;
                return;
            }

            if (CbCountry.SelectedValue == null)
            {
                TbError.Text = "Выберите страну!";
                TbError.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrEmpty(TbYear.Text) || string.IsNullOrEmpty(TbPrice.Text))
            {
                TbError.Text = "Заполните все поля!";
                TbError.Visibility = Visibility.Visible;
                return;
            }

            if (!int.TryParse(TbYear.Text, out int year) || year < 1900 || year > DateTime.Now.Year)
            {
                TbError.Text = "Некорректный год (1900-текущий)!";
                TbError.Visibility = Visibility.Visible;
                return;
            }

            if (!decimal.TryParse(TbPrice.Text, out decimal price) || price <= 0)
            {
                TbError.Text = "Некорректная цена!";
                TbError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                Cars car;
                if (_carId == 0)
                {
                    car = new Cars();
                    AppConnect.model01.Cars.Add(car);
                }
                else
                {
                    car = AppConnect.model01.Cars.Find(_carId);
                    if (car == null)
                    {
                        MessageBox.Show("Автомобиль не найден!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                car.IdBrand = (int)CbBrand.SelectedValue;
                car.IdCountry = (int)CbCountry.SelectedValue;
                car.Year = year;
                car.Price = price;
                car.ImageName = _currentImageName;

                AppConnect.model01.SaveChanges();

                MessageBox.Show("Сохранено успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                AppFrame.FrameMain.Navigate(new CarsPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new CarsPage());
        }
    }
}