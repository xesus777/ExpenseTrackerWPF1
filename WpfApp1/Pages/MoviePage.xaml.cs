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
using static System.Collections.Specialized.BitVector32;

namespace WpfApp1.Pages
{
    public partial class MoviePage : Page
    {
        private int movieId;

        public MoviePage(int movieId)
        {
            InitializeComponent();
            this.movieId = movieId;
            LoadMovie();
            LoadSessions();
        }

        private void LoadMovie()
        {
            var movie = Core.Context.Movies.FirstOrDefault(m => m.MovieID == movieId);
            if (movie != null)
            {
                var ageRating = Core.Context.AgeRatings.FirstOrDefault(ar => ar.AgeRatingID == movie.AgeRatingID);

                TitleText.Text = movie.Title;
                RatingText.Text = $"Рейтинг: {movie.Rating}";
                AgeRatingText.Text = $"Возрастной рейтинг: {ageRating?.AgeRatingCode}";
                DescriptionText.Text = $"Описание: {movie.Description}";

                if (!string.IsNullOrEmpty(movie.PosterPath))
                {
                    try
                    {
                        PosterImage.Source = new BitmapImage(new Uri(movie.PosterPath, UriKind.RelativeOrAbsolute));
                    }
                    catch
                    {
                        
                    }
                }

                var genres = from mg in Core.Context.MovieGenres
                             join g in Core.Context.Genres on mg.GenreID equals g.GenreID
                             where mg.MovieID == movieId
                             select g.GenreName;

                GenresText.Text = $"Жанры: {string.Join(", ", genres.ToList())}";
            }
        }

        private void LoadSessions()
        {
            var sessions = from s in Core.Context.Sessions
                           join m in Core.Context.Movies on s.MovieID equals m.MovieID
                           join h in Core.Context.Halls on s.HallID equals h.HallID
                           where s.MovieID == movieId
                           orderby s.SessionDate, s.SessionTime
                           select new
                           {
                               s.SessionID,
                               s.SessionDate,
                               s.SessionTime,
                               s.Price,
                               h.HallName,
                               h.HallRating
                           };

            SessionsList.ItemsSource = sessions.ToList();
        }

        private void SelectSession_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUser == null)
            {
                MessageBox.Show("Необходимо войти в систему");
                NavigationService.Navigate(new LoginPage());
                return;
            }

            Button btn = sender as Button;
            int sessionId = (int)btn.Tag;
            NavigationService.Navigate(new SessionPage(sessionId));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
