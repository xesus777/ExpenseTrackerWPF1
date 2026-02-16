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
        private Users currentUser;

        public MoviePage(int movieId, Users user)
        {
            InitializeComponent();
            this.movieId = movieId;
            this.currentUser = user;

            LoadMovieInfo();
            LoadSessions();
        }

        private void LoadMovieInfo()
        {
            var movie = Core.Context.Movies
                .FirstOrDefault(m => m.MovieID == movieId);

            if (movie != null)
            {
                txtTitle.Text = movie.Title;
                txtDescription.Text = movie.Description;
                txtRating.Text = movie.Rating?.ToString("F1") ?? "N/A";

                if (movie.AgeRatings != null)
                {
                    txtAgeRating.Text = movie.AgeRatings.AgeRatingCode;
                }

                txtStartDate.Text = movie.StartDate?.ToString("dd.MM.yyyy") ?? "N/A";

                
                var genres = Core.Context.MovieGenres
                    .Where(mg => mg.MovieID == movieId)
                    .ToList();

                listGenres.ItemsSource = genres;
            }
        }

        private void LoadSessions()
        {
            var sessions = Core.Context.Sessions
                .Where(s => s.MovieID == movieId)
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.SessionTime)
                .ToList();

            listSessions.ItemsSource = sessions;
        }

        private void listSessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSessions.SelectedItem is Sessions selectedSession)
            {
                if (currentUser == null)
                {
                    MessageBox.Show("You need to login to book tickets");
                    (Application.Current.MainWindow as MainWindow)?.NavigateToLoginWithSession(selectedSession.SessionID);
                }
                else
                {
                    (Application.Current.MainWindow as MainWindow)?.NavigateToSession(selectedSession.SessionID, currentUser.UserID);
                }
            }
        }
    }
}
