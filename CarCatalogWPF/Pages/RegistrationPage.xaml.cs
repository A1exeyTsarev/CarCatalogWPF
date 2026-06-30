using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarCatalogWPF.AppData;

namespace CarCatalogWPF.Pages
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        // ===== ПРОВЕРКА: существует ли пользователь с таким логином =====
        private bool IsUserAlreadyExists(string login)
        {
            return AppConnect.model01.Users.Any(u => u.UserFullName == login);
        }

        // ===== КНОПКА "ЗАРЕГИСТРИРОВАТЬСЯ" =====
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка: все ли поля заполнены
            if (string.IsNullOrEmpty(TbLogin.Text) ||
                string.IsNullOrEmpty(PbPassword.Password) ||
                string.IsNullOrEmpty(PbPasswordRepeat.Password))
            {
                MessageBox.Show("❌ Заполните все поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Проверка: пароли совпадают
            if (PbPassword.Password != PbPasswordRepeat.Password)
            {
                MessageBox.Show("❌ Пароли не совпадают!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Проверка: существует ли пользователь с таким логином
            if (IsUserAlreadyExists(TbLogin.Text))
            {
                MessageBox.Show("❌ Такой логин уже существует!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 4. Создаем нового пользователя
                var newUser = new Users
                {
                    UserFullName = TbLogin.Text,
                    Password = PbPassword.Password,
                    IdRole = 1 // Client (роль по умолчанию)
                };

                // 5. Добавляем в БД
                AppConnect.model01.Users.Add(newUser);
                AppConnect.model01.SaveChanges();

                // 6. Сообщение об успехе
                MessageBox.Show("✅ Регистрация успешна! Теперь войдите.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // 7. Возврат на страницу входа
                AppFrame.FrameMain.Navigate(new LoginPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== КНОПКА "НАЗАД" =====
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new LoginPage());
        }

        // ===== СОБЫТИЕ TextChanged (если нужно) =====
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Можно оставить пустым, если не нужна логика
        }
    }
}