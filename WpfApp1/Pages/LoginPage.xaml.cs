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
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }


        public bool Auth(string login, string password)
        {
            // 1. Проверка на null
            if (login == null || password == null)
                return false;

            // 2. Проверка на пустые строки и строки из пробелов
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return false;

            // 3. Проверка на очень длинные строки (больше 50 символов)
            if (login.Length > 50 || password.Length > 50)
                return false;

            

            // 4. Поиск пользователя в БД
            var user = Core.Context.Users
                .FirstOrDefault(u => u.Username == login && u.Password == password);

            

            // Возвращаем true только если пользователь найден
            return user != null;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;

            if (Auth(login, password))
            {
                var user = Core.Context.Users.FirstOrDefault(u => u.Username == login && u.Password == password);
                MainWindow.CurrentUser = user;
                NavigationService.Navigate(new MainPage());
            }
            else
            {
                MessageText.Text = "Неверный логин или пароль";
            }
        }

        private void ToRegisterPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
