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
        public ProfilePage()
        {
            InitializeComponent();
            LoadUserInfo();
            LoadUserBookings();
        }

        private void LoadUserInfo()
        {
            var user = MainWindow.CurrentUser;
            if (user != null)
            {
                UsernameText.Text = $"Логин: {user.Username}";
                EmailText.Text = $"Email: {user.Email ?? "не указан"}";
                FullNameText.Text = $"Полное имя: {user.FullName ?? "не указано"}";
                RegDateText.Text = $"Дата регистрации: {user.RegistrationDate:dd.MM.yyyy}";
            }
        }

        private void LoadUserBookings()
        {
            var bookings = from b in Core.Context.Bookings
                           join s in Core.Context.Sessions on b.SessionID equals s.SessionID
                           join m in Core.Context.Movies on s.MovieID equals m.MovieID
                           join h in Core.Context.Halls on s.HallID equals h.HallID
                           join se in Core.Context.Seats on b.SeatID equals se.SeatID
                           where b.UserID == MainWindow.CurrentUser.UserID && b.IsActive == true
                           orderby s.SessionDate descending, s.SessionTime descending
                           select new
                           {
                               Title = m.Title,
                               SessionDate = s.SessionDate,
                               SessionTime = s.SessionTime,
                               HallName = h.HallName,
                               SeatRow = se.SeatRow,
                               SeatNumber = se.SeatNumber,
                               Price = s.Price
                           };

            BookingsList.ItemsSource = bookings.ToList();
        }

        private void Back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
