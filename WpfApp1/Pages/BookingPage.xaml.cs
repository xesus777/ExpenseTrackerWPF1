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
    public partial class BookingPage : Page
    {
        private int sessionId;
        private int userId;
        private int seatId;

        public BookingPage(int sessionId, int userId, int seatId)
        {
            InitializeComponent();
            this.sessionId = sessionId;
            this.userId = userId;
            this.seatId = seatId;

            LoadBookingInfo();
        }

        private void LoadBookingInfo()
        {
            var session = Core.Context.Sessions.Find(sessionId);
            if (session == null) return;

            var movie = Core.Context.Movies.Find(session.MovieID);
            var seat = Core.Context.Seats.Find(seatId);

            if (session != null && movie != null && seat != null)
            {
                txtMovie.Text = movie.Title;
                txtHall.Text = session.Halls?.HallName;
                txtDateTime.Text = $"{session.SessionDate:dd.MM.yyyy} {session.SessionTime}";
                txtSeat.Text = $"Row {seat.SeatRow} Seat {seat.SeatNumber}";
                txtPrice.Text = $"{session.Price} RUB";
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var existingBooking = Core.Context.Bookings
                    .FirstOrDefault(b => b.SessionID == sessionId && b.SeatID == seatId && b.IsActive == true);

                if (existingBooking != null)
                {
                    MessageBox.Show("This seat was just taken. Please select another.");
                    (Application.Current.MainWindow as MainWindow)?.NavigateToSession(sessionId, userId);
                    return;
                }

                Bookings newBooking = new Bookings
                {
                    UserID = userId,
                    SessionID = sessionId,
                    SeatID = seatId,
                    BookingDate = DateTime.Now,
                    IsActive = true
                };

                Core.Context.Bookings.Add(newBooking);
                Core.Context.SaveChanges();

                MessageBox.Show("Ticket booked successfully!");
                (Application.Current.MainWindow as MainWindow)?.NavigateToMain();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error booking ticket: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.NavigateToSession(sessionId, userId);
        }
    }
}
