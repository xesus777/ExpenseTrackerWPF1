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
    public partial class MainPage : Page
    {
        private Users currentUser;

        public MainPage()
        {
            InitializeComponent();
            LoadMovies();
        }

        public void SetCurrentUser(Users user)
        {
            currentUser = user;
            UpdateLoginUI();
        }

        private void LoadMovies()
        {
            var movies = Core.Context.Movies.Include("AgeRatings").ToList();
            listMovies.ItemsSource = movies;
        }

        private void UpdateLoginUI()
        {
            if (currentUser != null)
            {
                btnLogin.Visibility = Visibility.Collapsed;
                btnRegister.Visibility = Visibility.Collapsed;
                btnProfile.Visibility = Visibility.Visible;
                btnLogout.Visibility = Visibility.Visible;
            }
            else
            {
                btnLogin.Visibility = Visibility.Visible;
                btnRegister.Visibility = Visibility.Visible;
                btnProfile.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                listMovies.ItemsSource = Core.Context.Movies.Include("AgeRatings").ToList();
            }
            else
            {
                var searchResult = Core.Context.Movies.Include("AgeRatings")
                    .Where(m => m.Title.Contains(txtSearch.Text))
                    .ToList();
                listMovies.ItemsSource = searchResult;
            }
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbSort.SelectedIndex)
            {
                case 0:
                    listMovies.ItemsSource = Core.Context.Movies.Include("AgeRatings")
                        .OrderBy(m => m.Title).ToList();
                    break;
                case 1:
                    listMovies.ItemsSource = Core.Context.Movies.Include("AgeRatings")
                        .OrderBy(m => m.Rating).ToList();
                    break;
                case 2:
                    listMovies.ItemsSource = Core.Context.Movies.Include("AgeRatings")
                        .OrderByDescending(m => m.Rating).ToList();
                    break;
            }
        }

        private void listMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listMovies.SelectedItem is Movies selectedMovie)
            {
                (Application.Current.MainWindow as MainWindow)?.NavigateToMovie(selectedMovie.MovieID, currentUser);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.NavigateToLogin();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.NavigateToRegister();
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser != null)
            {
                (Application.Current.MainWindow as MainWindow)?.NavigateToProfile(currentUser.UserID);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.Logout();
        }
    }
}
