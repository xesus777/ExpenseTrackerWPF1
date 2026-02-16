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
    public partial class ProfilePage : Page
    {
        private int userId;

        public ProfilePage(int userId)
        {
            InitializeComponent();
            this.userId = userId;

            LoadUserInfo();
            LoadUserBookings();
        }

        private void LoadUserInfo()
        {
            var user = Core.Context.Users.Find(userId);
            if (user != null)
            {
                txtUserName.Text = $"Username: {user.Username}";
                txtUserEmail.Text = $"Email: {user.Email}";
                txtUserFullName.Text = $"Full name: {user.FullName}";
            }
        }

        private void LoadUserBookings()
        {
            var bookings = Core.Context.Bookings
                .Where(b => b.UserID == userId && b.IsActive == true)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            var bookingsForDisplay = bookings.Select(b => new
            {
                b.BookingID,
                MovieTitle = b.Sessions?.Movies?.Title ?? "Unknown",
                HallName = b.Sessions?.Halls?.HallName ?? "Unknown",
                SessionDate = b.Sessions?.SessionDate,
                SessionTime = b.Sessions?.SessionTime,
                SeatRow = b.Seats?.SeatRow,
                SeatNumber = b.Seats?.SeatNumber,
                Price = b.Sessions?.Price ?? 0
            }).ToList();

            listBookings.ItemsSource = bookingsForDisplay;
        }
    }
}
