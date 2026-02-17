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
        private int seatId;
        private string movieTitle;
        private string hallName;
        private DateTime sessionDate;
        private TimeSpan sessionTime;
        private decimal price;
        private Seats seat;

        public BookingPage(int sessionId, int seatId)
        {
            InitializeComponent();
            this.sessionId = sessionId;
            this.seatId = seatId;
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
                               m.Title,
                               h.HallName,
                               s.SessionDate,
                               s.SessionTime,
                               s.Price
                           }).FirstOrDefault();

            seat = Core.Context.Seats.FirstOrDefault(s => s.SeatID == seatId);

            if (session != null && seat != null)
            {
                movieTitle = session.Title;
                hallName = session.HallName;
                sessionDate = session.SessionDate;
                sessionTime = session.SessionTime;
                price = session.Price;

                MovieText.Text = $"Фильм: {movieTitle}";
                HallText.Text = $"Зал: {hallName}";
                DateTimeText.Text = $"Дата и время: {sessionDate:dd.MM.yyyy} {sessionTime}";
                SeatText.Text = $"Место: {seat.SeatRow} ряд, {seat.SeatNumber} место";
                PriceText.Text = $"Цена: {price} руб.";
            }
        }

        private void ConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            bool isBooked = Core.Context.Bookings.Any(b => b.SessionID == sessionId && b.SeatID == seatId && b.IsActive == true);

            if (isBooked)
            {
                MessageBox.Show("Извините, это место уже занято");
                NavigationService.Navigate(new MainPage());
                return;
            }

            Bookings newBooking = new Bookings
            {
                UserID = MainWindow.CurrentUser.UserID,
                SessionID = sessionId,
                SeatID = seatId,
                BookingDate = DateTime.Now,
                IsActive = true
            };

            Core.Context.Bookings.Add(newBooking);
            Core.Context.SaveChanges();

            MessageBox.Show("Билет успешно забронирован!");
            NavigationService.Navigate(new MainPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
