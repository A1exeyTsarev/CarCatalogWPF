using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarCatalogWPF.AppData;

namespace CarCatalogWPF.Pages
{
    public partial class CarsPage : Page
    {
        public CarsPage()
        {
            InitializeComponent();
            InitializePage();
        }

        private void InitializePage()
        {
            try
            {
                LoadFilters();
                LoadData();
                SetPermissions();
                UpdateCounter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки страницы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFilters()
        {
            var countries = AppConnect.model01.Countries.Select(c => c.NameCountry).Distinct().ToList();
            countries.Insert(0, "Все страны");
            ComboFilter.ItemsSource = countries;
            ComboFilter.SelectedIndex = 0;

            ComboSort.ItemsSource = new string[]
            {
                "Без сортировки",
                "По цене ↑",
                "По цене ↓",
                "По году ↑",
                "По году ↓"
            };
            ComboSort.SelectedIndex = 0;
        }

        // ===== ИСПРАВЛЕННЫЙ МЕТОД LoadData =====
        private void LoadData()
        {
            try
            {
                // 1. Сначала загружаем данные из БД в память через ToList()
                var carsFromDb = AppConnect.model01.Cars.ToList();

                // 2. Потом формируем ViewModel (здесь уже можно использовать $)
                var cars = carsFromDb.Select(c => new CarViewModel
                {
                    IdCar = c.IdCar,
                    BrandName = c.Brands.BrandName,
                    CountryName = c.Countries.NameCountry,
                    Year = c.Year,
                    Price = c.Price,
                    ImagePath = $"/Images/{c.ImageName}" // ← Теперь работает!
                }).ToList();

                LvCars.ItemsSource = cars;
                UpdateCounter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== ИСПРАВЛЕННЫЙ МЕТОД GetFilteredCars =====
        private CarViewModel[] GetFilteredCars()
        {
            try
            {
                // 1. Сначала загружаем данные из БД в память через ToList()
                var carsFromDb = AppConnect.model01.Cars.ToList();

                // 2. Потом формируем ViewModel
                var cars = carsFromDb.Select(c => new CarViewModel
                {
                    IdCar = c.IdCar,
                    BrandName = c.Brands.BrandName,
                    CountryName = c.Countries.NameCountry,
                    Year = c.Year,
                    Price = c.Price,
                    ImagePath = $"/Images/{c.ImageName}"
                }).ToList();

                // 3. Поиск (работает в памяти)
                if (!string.IsNullOrWhiteSpace(TextSearch.Text))
                {
                    string search = TextSearch.Text.ToLower();
                    cars = cars.Where(x =>
                        x.BrandName.ToLower().Contains(search) ||
                        x.CountryName.ToLower().Contains(search)
                    ).ToList();
                }

                // 4. Фильтр по стране
                if (ComboFilter.SelectedIndex > 0)
                {
                    string selectedCountry = ComboFilter.SelectedItem.ToString();
                    cars = cars.Where(x => x.CountryName == selectedCountry).ToList();
                }

                // 5. Сортировка
                switch (ComboSort.SelectedIndex)
                {
                    case 1: cars = cars.OrderBy(x => x.Price).ToList(); break;
                    case 2: cars = cars.OrderByDescending(x => x.Price).ToList(); break;
                    case 3: cars = cars.OrderBy(x => x.Year).ToList(); break;
                    case 4: cars = cars.OrderByDescending(x => x.Year).ToList(); break;
                    default: cars = cars.OrderBy(x => x.BrandName).ToList(); break;
                }

                return cars.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void SetPermissions()
        {
            string role = LoginPage.CurrentRole;

            switch (role)
            {
                case "Admin":
                    BtnAdd.Visibility = Visibility.Visible;
                    BtnEdit.Visibility = Visibility.Visible;
                    BtnDelete.Visibility = Visibility.Visible;
                    break;
                case "Manager":
                    BtnAdd.Visibility = Visibility.Collapsed;
                    BtnEdit.Visibility = Visibility.Visible;
                    BtnDelete.Visibility = Visibility.Collapsed;
                    break;
                default:
                    BtnAdd.Visibility = Visibility.Collapsed;
                    BtnEdit.Visibility = Visibility.Collapsed;
                    BtnDelete.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void UpdateCounter()
        {
            var items = LvCars.ItemsSource as System.Collections.IEnumerable;
            int count = items?.Cast<object>().Count() ?? 0;
            TbCounter.Text = $"🚗 Всего автомобилей: {count}";
        }

        private void ApplyFilters()
        {
            var filteredCars = GetFilteredCars();
            LvCars.ItemsSource = filteredCars;
            UpdateCounter();
        }

        private void TextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new AddEditCarPage(0));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = LvCars.SelectedItem as CarViewModel;
            if (selected == null)
            {
                MessageBox.Show("Выберите автомобиль!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AppFrame.FrameMain.Navigate(new AddEditCarPage(selected.IdCar));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = LvCars.SelectedItem as CarViewModel;
            if (selected == null)
            {
                MessageBox.Show("Выберите автомобиль!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить автомобиль '{selected.BrandName}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var car = AppConnect.model01.Cars.Find(selected.IdCar);
                    AppConnect.model01.Cars.Remove(car);
                    AppConnect.model01.SaveChanges();
                    LoadData();
                    MessageBox.Show("Автомобиль удален!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new LoginPage());
        }

        private void LvCars_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LvCars.SelectedItem is CarViewModel selectedCar)
            {
                if (LoginPage.CurrentRole != "Client")
                {
                    AppFrame.FrameMain.Navigate(new AddEditCarPage(selectedCar.IdCar));
                }
            }
        }
    }

    public class CarViewModel
    {
        public int IdCar { get; set; }
        public string BrandName { get; set; }
        public string CountryName { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    }
}