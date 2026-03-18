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

namespace WpfApp1.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        // Класс для результата регистрации
        public class RegistrationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public Users NewUser { get; set; }
        }

        // Метод для тестирования регистрации
        public RegistrationResult Register(string login, string password, string confirmPassword,
                                           string email, string fullName)
        {
            var result = new RegistrationResult();

            try
            {
                // 1. Проверка на пустые поля
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    result.Success = false;
                    result.Message = "Заполните логин и пароль";
                    return result;
                }

                // 2. Проверка совпадения паролей
                if (password != confirmPassword)
                {
                    result.Success = false;
                    result.Message = "Пароли не совпадают";
                    return result;
                }

                // 3. Проверка длины пароля (не менее 6 символов)
                if (password.Length < 6)
                {
                    result.Success = false;
                    result.Message = "Пароль должен быть не менее 6 символов";
                    return result;
                }

                // 4. Проверка длины логина (не менее 3 символов)
                if (login.Length < 3)
                {
                    result.Success = false;
                    result.Message = "Логин должен быть не менее 3 символов";
                    return result;
                }

                // 5. Проверка на существующего пользователя
                if (Core.Context.Users.Any(u => u.Username == login))
                {
                    result.Success = false;
                    result.Message = "Пользователь с таким логином уже существует";
                    return result;
                }

                // Создание нового пользователя
                Users newUser = new Users
                {
                    Username = login,
                    Password = password,
                    Email = email,
                    FullName = fullName,
                    RegistrationDate = DateTime.Now
                };

                Core.Context.Users.Add(newUser);
                Core.Context.SaveChanges();

                result.Success = true;
                result.Message = "Регистрация успешна";
                result.NewUser = newUser;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ошибка при сохранении: {ex.Message}";
            }

            return result;
        }

        // Обновленный обработчик кнопки регистрации
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var result = Register(
                LoginBox.Text,
                PasswordBox.Password,
                ConfirmPasswordBox.Password,
                EmailBox.Text,
                FullNameBox.Text
            );

            if (result.Success)
            {
                MessageBox.Show(result.Message);
                NavigationService.Navigate(new LoginPage());
            }
            else
            {
                MessageText.Text = result.Message;
            }
        }

        private void ToLoginPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
