using CarCatalogWPF.AppData;
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

using CarCatalogWPF.Pages;     // для доступа к страницам (LoginPage, CarsPage и т.д.)

namespace CarCatalogWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppConnect.model01 = new CarCatalogDBEntities();

            // Привязываем Frame из XAML к статическому полю
            AppFrame.FrameMain = Frame1;

            // Открываем страницу входа
            Frame1.Navigate(new LoginPage());
        }
    }
}
