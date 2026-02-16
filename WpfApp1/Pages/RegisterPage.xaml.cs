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

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Password))
            {
                MessageBox.Show("Fill all required fields");
                return;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            var existingUser = Core.Context.Users
                .FirstOrDefault(u => u.Username == txtUsername.Text);

            if (existingUser != null)
            {
                MessageBox.Show("Username already exists");
                return;
            }

            User newUser = new User
            {
                Username = txtUsername.Text,
                Password = txtPassword.Password,
                Email = txtEmail.Text,
                FullName = txtFullName.Text,
                RegistrationDate = DateTime.Now
            };

            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();

            MessageBox.Show("Registration successful! You can now login.");
            (Application.Current.MainWindow as MainWindow)?.NavigateToLogin();
        }
    }
}
