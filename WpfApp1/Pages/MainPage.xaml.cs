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
        public MainPage()
        {
            InitializeComponent();
            LoadMovies();
            UpdateUserUI();
        }

        private void LoadMovies()
        {
            var movies = from m in Core.Context.Movies
                         join ar in Core.Context.AgeRatings on m.AgeRatingID equals ar.AgeRatingID
                         select new
                         {
                             m.MovieID,
                             m.Title,
                             m.Rating,
                             m.StartDate,
                             m.PosterPath,
                             AgeRatingCode = ar.AgeRatingCode
                         };

            MoviesList.ItemsSource = movies.ToList();
        }

        private void UpdateUserUI()
        {
            if (MainWindow.CurrentUser != null)
            {
                UserInfoText.Text = $"Пользователь: {MainWindow.CurrentUser.Username}";
                LoginRegisterBtn.Visibility = Visibility.Collapsed;
                ProfileBtn.Visibility = Visibility.Visible;
                LogoutBtn.Visibility = Visibility.Visible;
            }
            else
            {
                UserInfoText.Text = "";
                LoginRegisterBtn.Visibility = Visibility.Visible;
                ProfileBtn.Visibility = Visibility.Collapsed;
                LogoutBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void ApplyFilterAndSort()
        {
            var query = from m in Core.Context.Movies
                        join ar in Core.Context.AgeRatings on m.AgeRatingID equals ar.AgeRatingID
                        select new
                        {
                            m.MovieID,
                            m.Title,
                            m.Rating,
                            m.StartDate,
                            m.PosterPath,
                            AgeRatingCode = ar.AgeRatingCode
                        };

            string search = SearchBox.Text?.ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.Title.ToLower().Contains(search));
            }

            var selectedSort = SortBox.SelectedItem as ComboBoxItem;
            if (selectedSort?.Tag?.ToString() == "Title")
            {
                query = query.OrderBy(m => m.Title);
            }
            else if (selectedSort?.Tag?.ToString() == "Rating")
            {
                query = query.OrderByDescending(m => m.Rating);
            }

            MoviesList.ItemsSource = query.ToList();
        }

        private void MovieDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int movieId = (int)btn.Tag;
            NavigationService.Navigate(new MoviePage(movieId));
        }

        private void LoginRegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUser != null)
                NavigationService.Navigate(new ProfilePage());
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CurrentUser = null;
            UpdateUserUI();
        }
    }
}
