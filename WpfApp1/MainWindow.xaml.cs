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
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public static int CurrentUserId { get; set; }
        public static string CurrentUserRole { get; set; }
        public static string CurrentUserName { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            MainFrame.Navigate(new StartPage());
            UpdateUIBasedOnAuth();
        }

        public void UpdateUIBasedOnAuth()
        {
            bool isAuth = CurrentUserId > 0;
            bool isClient = CurrentUserRole == "Клиент";

            BtnLogin.Visibility = isAuth ? Visibility.Collapsed : Visibility.Visible;
            BtnAccount.Visibility = isAuth ? Visibility.Visible : Visibility.Collapsed;
            BtnLogout.Visibility = isAuth ? Visibility.Visible : Visibility.Collapsed;

            BtnCart.Visibility = isClient ? Visibility.Visible : Visibility.Collapsed;

            BtnProducts.Visibility = Visibility.Visible;

            if (isAuth)
            {
                BtnAccount.Content = $"Аккаунт ({CurrentUserName})";
            }
        }

        public void NavigateTo(Page page)
        {
            MainFrame.Navigate(page);
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsPage());
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserId == 0)
            {
                MessageBox.Show("Для просмотра корзины необходимо авторизоваться");
                MainFrame.Navigate(new LoginPage());
                return;
            }
            MainFrame.Navigate(new CartPage());
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());
        }

        private void BtnAccount_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserRole == "Клиент")
                MainFrame.Navigate(new ClientAccountPage());
            else if (CurrentUserRole == "Мастер")
                MainFrame.Navigate(new MasterPage());
            else if (CurrentUserRole == "Менеджер")
                MainFrame.Navigate(new ManagerPage());
            else if (CurrentUserRole == "Администратор")
                MainFrame.Navigate(new AdminPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUserId = 0;
            CurrentUserRole = null;
            CurrentUserName = null;
            UpdateUIBasedOnAuth();
            MainFrame.Navigate(new StartPage());
            MessageBox.Show("Вы вышли из аккаунта", "Выход", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
