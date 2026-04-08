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
using System.Xml.Linq;

namespace WpfApp1.Pages
{
    public partial class AppointmentDetailsPage : Page
    {
        private int _appointmentId;

        public AppointmentDetailsPage(int appointmentId)
        {
            InitializeComponent();
            _appointmentId = appointmentId;
            LoadData();
        }

        private void LoadData()
        {
            var appointment = (from a in Core.Context.Appointments
                               join st in Core.Context.ServiceTypes on a.ServiceTypeId equals st.Id
                               join c in Core.Context.Users on a.ClientId equals c.Id
                               where a.Id == _appointmentId
                               select new
                               {
                                   a.AppointmentDateTime,
                                   ClientName = c.FullName,
                                   ClientPhone = c.Phone,
                                   ServiceName = st.Name,
                                   a.Price,
                                   a.Comment,
                                   a.Status
                               }).FirstOrDefault();

            if (appointment != null)
            {
                TxtDateTime.Text = appointment.AppointmentDateTime.ToString("dd.MM.yyyy HH:mm");
                TxtClientName.Text = appointment.ClientName;
                TxtClientPhone.Text = appointment.ClientPhone;
                TxtService.Text = appointment.ServiceName;
                TxtPrice.Text = $"{appointment.Price:F2} руб";
                TxtComment.Text = string.IsNullOrEmpty(appointment.Comment) ? "Нет комментария" : appointment.Comment;
                TxtStatus.Text = GetStatusText(appointment.Status);

                if (appointment.Status == "Scheduled")
                {
                    BtnComplete.Visibility = Visibility.Visible;
                }
            }
        }

        private string GetStatusText(string status)
        {
            switch (status)
            {
                case "Scheduled": return "Запланирована";
                case "Completed": return "Выполнена";
                case "Cancelled": return "Отменена";
                default: return status;
            }
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Отметить запись как выполненную?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var appointment = Core.Context.Appointments.FirstOrDefault(a => a.Id == _appointmentId);
                if (appointment != null)
                {
                    appointment.Status = "Completed";
                    Core.Context.SaveChanges();
                    MessageBox.Show("Запись отмечена как выполненная");
                    NavigationService.GoBack();
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
