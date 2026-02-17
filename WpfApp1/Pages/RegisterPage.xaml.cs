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

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string email = EmailBox.Text;
            string fullName = FullNameBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageText.Text = "Заполните логин и пароль";
                return;
            }

            if (password != confirmPassword)
            {
                MessageText.Text = "Пароли не совпадают";
                return;
            }

            if (Core.Context.Users.Any(u => u.Username == login))
            {
                MessageText.Text = "Пользователь с таким логином уже существует";
                return;
            }

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

            MessageBox.Show("Регистрация успешна! Теперь войдите в систему.");
            NavigationService.Navigate(new LoginPage());
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
