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
    public partial class SessionPage : Page
    {
        private int sessionId;
        private int userId;
        private int movieId;
        private dynamic selectedSeat; 

        public SessionPage(int sessionId, int userId)
        {
            InitializeComponent();
            this.sessionId = sessionId;
            this.userId = userId;

            LoadSessionInfo();
            LoadSeats();
        }

        private void LoadSessionInfo()
        {
            var session = Core.Context.Sessions.Find(sessionId);
            if (session == null) return;

            var movie = Core.Context.Movies.Find(session.MovieID);

            if (session != null)
            {
                movieId = session.MovieID ?? 0;
                txtSessionInfo.Text = $"Session: {session.SessionDate:dd.MM.yyyy} {session.SessionTime} - Hall: {session.Halls?.HallName}";
                txtPrice.Text = $"Price: {session.Price} RUB";
            }
        }

        private void LoadSeats()
        {
            var session = Core.Context.Sessions.Find(sessionId);
            if (session == null) return;

            var allSeats = Core.Context.Seats
                .Where(s => s.HallID == session.HallID)
                .OrderBy(s => s.SeatRow)
                .ThenBy(s => s.SeatNumber)
                .ToList();

            var bookedSeatIds = Core.Context.Bookings
                .Where(b => b.SessionID == sessionId && b.IsActive == true)
                .Select(b => b.SeatID)
                .ToList();

            
            var seatsForDisplay = allSeats.Select(seat => new
            {
                seat.SeatID,
                seat.SeatRow,
                seat.SeatNumber,
                seat.IsAvailable,
                IsBooked = bookedSeatIds.Contains(seat.SeatID)
            }).ToList();

            seatsContainer.ItemsSource = seatsForDisplay;
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic seat = btn.DataContext; 

            if (seat.IsBooked)
            {
                MessageBox.Show("This seat is already taken");
                return;
            }

            if (selectedSeat != null)
            {
                
                foreach (var item in seatsContainer.Items)
                {
                    if (item != null && (int)item.GetType().GetProperty("SeatID").GetValue(item) == (int)selectedSeat.GetType().GetProperty("SeatID").GetValue(selectedSeat))
                    {
                        var container = seatsContainer.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                        if (container != null)
                        {
                            var button = FindVisualChild<Button>(container);
                            if (button != null)
                                button.Background = new SolidColorBrush(Colors.LightGreen);
                        }
                        break;
                    }
                }
            }

            if (selectedSeat != null && (int)selectedSeat.GetType().GetProperty("SeatID").GetValue(selectedSeat) == (int)seat.GetType().GetProperty("SeatID").GetValue(seat))
            {
                selectedSeat = null;
                btn.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                selectedSeat = seat;
                btn.Background = new SolidColorBrush(Colors.Yellow);
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    var result = FindVisualChild<T>(child);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            selectedSeat = null;
            LoadSeats();
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSeat == null)
            {
                MessageBox.Show("Select a seat");
                return;
            }

            int seatId = (int)selectedSeat.GetType().GetProperty("SeatID").GetValue(selectedSeat);

            var existingBooking = Core.Context.Bookings
                .FirstOrDefault(b => b.SessionID == sessionId && b.SeatID == seatId && b.IsActive == true);

            if (existingBooking != null)
            {
                MessageBox.Show("This seat was just taken. Please select another.");
                LoadSeats();
                return;
            }

            (Application.Current.MainWindow as MainWindow)?.NavigateToBooking(sessionId, userId, seatId);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var user = Core.Context.Users.Find(userId);
            (Application.Current.MainWindow as MainWindow)?.NavigateToMovie(movieId, user);
        }
    }
}
