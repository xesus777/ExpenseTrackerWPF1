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
        public User LoggedUser { get; private set; }
        public int ReturnSessionId { get; set; }

        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Enter username and password");
                return;
            }

            var user = Core.Context.Users
                .FirstOrDefault(u => u.Username == txtUsername.Text && u.Password == txtPassword.Password);

            if (user != null)
            {
                LoggedUser = user;
                (Application.Current.MainWindow as MainWindow)?.CloseLoginPage(this);
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.NavigateToRegister();
        }
    }
}
