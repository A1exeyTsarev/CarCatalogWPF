using CarCatalogWPF.AppData;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarCatalogWPF.Pages
{
    public partial class LoginPage : Page
    {
        public static int CurrentUserId;
        public static string CurrentRole;

        public LoginPage()
        {
            InitializeComponent();
        }

        // КНОПКА "ВОЙТИ" 
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ищем пользователя в БД (как в методичке)
                var userObj = AppConnect.model01.Users.FirstOrDefault(x =>
                    x.UserFullName == TbLogin.Text && x.Password == PbPassword.Password);

                if (userObj == null)
                {
                    MessageBox.Show("Такого пользователя нет", "Ошибка авторизации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Сохраняем данные пользователя
                    CurrentUserId = userObj.IdUser;
                    CurrentRole = userObj.Roles.NameRole;

                    MessageBox.Show($"Здравствуйте, {userObj.UserFullName}!", "Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Переходим на страницу с автомобилями
                    AppFrame.FrameMain.Navigate(new CarsPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message.ToString(), "Критическая ошибка приложения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppFrame.FrameMain.Navigate(new RegistrationPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}