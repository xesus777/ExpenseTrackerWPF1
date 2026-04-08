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
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            DatePickerSlots.SelectedDate = DateTime.Now.Date;
            LoadMastersServices();
        }

        private void LoadMastersServices()
        {
            var masters = Core.Context.Users
                .Where(u => u.RoleId == 2 && (u.IsFrozen == false || u.IsFrozen == null))
                .ToList();

            var masterServicesList = new List<MasterServiceInfo>();

            foreach (var master in masters)
            {
                var services = (from ms in Core.Context.MasterServices
                                join st in Core.Context.ServiceTypes on ms.ServiceTypeId equals st.Id
                                where ms.MasterId == master.Id
                                select new { st.Id, st.Name, st.Price, st.Duration }).ToList();

                if (services.Any())
                {
                    masterServicesList.Add(new MasterServiceInfo
                    {
                        MasterId = master.Id,
                        MasterName = master.FullName,
                        ServicesList = string.Join(", ", services.Select(s => s.Name)),
                        PricesList = string.Join(", ", services.Select(s => $"{s.Price} руб")),
                        DurationList = string.Join(", ", services.Select(s => $"{s.Duration} мин")),
                        AvailableSlots = GetAvailableSlotsForMaster(master.Id, DatePickerSlots.SelectedDate ?? DateTime.Now.Date)
                    });
                }
            }

            LvMastersServices.ItemsSource = masterServicesList;
        }

        private string GetAvailableSlotsForMaster(int masterId, DateTime date)
        {
            var existingAppointments = Core.Context.Appointments
                .Where(a => a.MasterId == masterId && a.Status != "Cancelled")
                .Select(a => a.AppointmentDateTime)
                .ToList();

            var freeSlots = new List<string>();

            for (int hour = 10; hour <= 18; hour++)
            {
                var slotTime = date.Date.AddHours(hour);
                if (!existingAppointments.Contains(slotTime) && slotTime > DateTime.Now)
                {
                    freeSlots.Add($"{hour}:00");
                }
            }

            if (!freeSlots.Any())
                return "Нет свободных записей";

            return string.Join(", ", freeSlots);
        }

        private void DatePickerSlots_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadMastersServices();
        }
    }

    public class MasterServiceInfo
    {
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public string ServicesList { get; set; }
        public string PricesList { get; set; }
        public string DurationList { get; set; }
        public string AvailableSlots { get; set; }
    }
}
