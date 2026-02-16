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
using System.Windows.Shapes;
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public Users CurrentUser { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            NavigateToMain();
        }

        public void NavigateToMain()
        {
            MainPage mainPage = new MainPage();
            mainPage.SetCurrentUser(CurrentUser);
            MainFrame.Navigate(mainPage);
        }

        public void NavigateToLogin()
        {
            LoginPage loginPage = new LoginPage();
            MainFrame.Navigate(loginPage);
        }

        public void NavigateToLoginWithSession(int sessionId)
        {
            LoginPage loginPage = new LoginPage();
            loginPage.ReturnSessionId = sessionId;
            MainFrame.Navigate(loginPage);
        }

        public void NavigateToRegister()
        {
            RegisterPage registerPage = new RegisterPage();
            MainFrame.Navigate(registerPage);
        }

        public void NavigateToMovie(int movieId, Users user)
        {
            MoviePage moviePage = new MoviePage(movieId, user);
            MainFrame.Navigate(moviePage);
        }

        public void NavigateToSession(int sessionId, int userId)
        {
            SessionPage sessionPage = new SessionPage(sessionId, userId);
            MainFrame.Navigate(sessionPage);
        }

        public void NavigateToBooking(int sessionId, int userId, int seatId)
        {
            BookingPage bookingPage = new BookingPage(sessionId, userId, seatId);
            MainFrame.Navigate(bookingPage);
        }

        public void NavigateToProfile(int userId)
        {
            ProfilePage profilePage = new ProfilePage(userId);
            MainFrame.Navigate(profilePage);
        }

        public void CloseLoginPage(LoginPage loginPage)
        {
            CurrentUser = loginPage.LoggedUser;

            if (loginPage.ReturnSessionId > 0)
            {
                NavigateToSession(loginPage.ReturnSessionId, CurrentUser.UserID);
            }
            else
            {
                NavigateToMain();
            }
        }

        public void Logout()
        {
            CurrentUser = null;
            NavigateToMain();
        }
    }
}
