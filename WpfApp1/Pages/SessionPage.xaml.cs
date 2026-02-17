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
        private int hallId;
        private string movieTitle;
        private string hallName;
        private DateTime sessionDate;
        private TimeSpan sessionTime;
        private decimal price;
        private List<Seats> allSeats;
        private List<int> bookedSeatIds;
        private Seats selectedSeat = null;

        public SessionPage(int sessionId)
        {
            InitializeComponent();

            if (MainWindow.CurrentUser == null)
            {
                NavigationService.Navigate(new LoginPage());
                return;
            }

            this.sessionId = sessionId;
            LoadData();
            
        }

        private void LoadData()
        {
            var session = (from s in Core.Context.Sessions
                           join m in Core.Context.Movies on s.MovieID equals m.MovieID
                           join h in Core.Context.Halls on s.HallID equals h.HallID
                           where s.SessionID == sessionId
                           select new
                           {
                               s.SessionID,
                               s.SessionDate,
                               s.SessionTime,
                               s.Price,
                               s.HallID,
                               m.Title,
                               h.HallName
                           }).FirstOrDefault();

            if (session != null)
            {
                hallId = session.HallID??0;
                movieTitle = session.Title;
                hallName = session.HallName;
                sessionDate = session.SessionDate;
                sessionTime = session.SessionTime;
                price = session.Price;

                SessionInfo.Text = $"{movieTitle} - Зал {hallName} - {sessionDate:dd.MM.yyyy} {sessionTime}";
            }

            LoadBookedSeats();
            allSeats = Core.Context.Seats
                .Where(s => s.HallID == hallId)
                .OrderBy(s => s.SeatRow)
                .ThenBy(s => s.SeatNumber)
                .ToList();

            UpdateSeatsDisplay();
        }

        private void LoadBookedSeats()
        {
            bookedSeatIds = Core.Context.Bookings
                .Where(b => b.SessionID == sessionId && b.IsActive == true)
                .Select(b => b.SeatID)
                .ToList()
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();
        }

        private void UpdateSeatsDisplay()
        {
            var seatsToShow = allSeats.Select(s => new
            {
                SeatID = s.SeatID,
                SeatNumber = $"{s.SeatRow}-{s.SeatNumber}",
                IsAvailable = !bookedSeatIds.Contains(s.SeatID)
            }).ToList();

            SeatsGrid.ItemsSource = seatsToShow;
        }

        private void ClearAllBookedSeats_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить все занятые места на этом сеансе?",
                                        "Подтверждение",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var bookingsToDelete = Core.Context.Bookings
                    .Where(b => b.SessionID == sessionId && b.IsActive == true)
                    .ToList();

                foreach (var booking in bookingsToDelete)
                {
                    booking.IsActive = false;
                }

                Core.Context.SaveChanges();

                LoadBookedSeats();
                UpdateSeatsDisplay();
                ClearSelectedSeat();

                MessageBox.Show("Все места очищены!");
            }
        }

        private void ClearSelectedSeat()
        {
            selectedSeat = null;
            SelectedSeatText.Text = "";
            BookButton.IsEnabled = false;
        }

        private void SelectSeat_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int seatId = (int)btn.Tag;

            if (bookedSeatIds.Contains(seatId))
            {
                MessageBox.Show("Это место уже занято");
                return;
            }

            selectedSeat = allSeats.First(s => s.SeatID == seatId);
            SelectedSeatText.Text = $"Выбрано место: {selectedSeat.SeatRow}-{selectedSeat.SeatNumber}";
            BookButton.IsEnabled = true;
        }

        private void BookTicket_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSeat != null)
            {
                NavigationService.Navigate(new BookingPage(sessionId, selectedSeat.SeatID));
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isAvailable = (bool)value;
            return isAvailable ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
