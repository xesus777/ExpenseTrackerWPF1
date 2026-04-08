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
    public partial class AppointmentsCatalogPage : Page
    {
        private List<SlotDisplayInfo> _availableSlots;

        public AppointmentsCatalogPage()
        {
            InitializeComponent();
            LoadMasters();
            DatePickerFilter.SelectedDate = DateTime.Now.Date;
        }

        private void LoadMasters()
        {
            var masters = Core.Context.Users
                .Where(u => u.RoleId == 2 && (u.IsFrozen == false || u.IsFrozen == null))
                .ToList();
            CmbMasters.ItemsSource = masters;
        }

        private void LoadServicesForMaster(int masterId)
        {
            var services = (from ms in Core.Context.MasterServices
                            join st in Core.Context.ServiceTypes on ms.ServiceTypeId equals st.Id
                            where ms.MasterId == masterId
                            select st).ToList();

            CmbServices.ItemsSource = services;
            CmbServices.IsEnabled = services.Any();

            if (services.Any())
            {
                CmbServices.SelectedIndex = 0;
            }
            else
            {
                CmbServices.ItemsSource = null;
                LvAvailableSlots.ItemsSource = null;
            }
        }

        private void CmbMasters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbMasters.SelectedValue != null)
            {
                int masterId = (int)CmbMasters.SelectedValue;
                LoadServicesForMaster(masterId);
            }
            else
            {
                CmbServices.ItemsSource = null;
                CmbServices.IsEnabled = false;
            }
        }

        private void CmbServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAvailableSlots();
        }

        private void DatePickerFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAvailableSlots();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadAvailableSlots();
        }

        private void LoadAvailableSlots()
        {
            if (CmbMasters.SelectedValue == null || CmbServices.SelectedValue == null) return;
            if (DatePickerFilter.SelectedDate == null) return;

            int masterId = (int)CmbMasters.SelectedValue;
            int serviceId = (int)CmbServices.SelectedValue;
            DateTime selectedDate = DatePickerFilter.SelectedDate.Value;

            var service = Core.Context.ServiceTypes.FirstOrDefault(s => s.Id == serviceId);
            var master = (Users)CmbMasters.SelectedItem;

            var existingAppointments = Core.Context.Appointments
                .Where(a => a.MasterId == masterId && a.Status != "Cancelled")
                .Select(a => a.AppointmentDateTime)
                .ToList();

            _availableSlots = new List<SlotDisplayInfo>();

            for (int hour = 10; hour <= 18; hour++)
            {
                var slotTime = selectedDate.Date.AddHours(hour);
                if (!existingAppointments.Contains(slotTime) && slotTime > DateTime.Now)
                {
                    _availableSlots.Add(new SlotDisplayInfo
                    {
                        DateTime = slotTime,
                        TimeDisplay = slotTime.ToString("dd.MM.yyyy HH:mm"),
                        ServiceName = service.Name,
                        ServiceId = serviceId,
                        MasterName = master.FullName,
                        MasterId = masterId,
                        Price = service.Price
                    });
                }
            }

            LvAvailableSlots.ItemsSource = _availableSlots;
        }

        private void LvAvailableSlots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvAvailableSlots.SelectedItem == null) return;

            if (MainWindow.CurrentUserId == 0)
            {
                var result = MessageBox.Show("Для записи необходимо авторизоваться. Перейти на страницу входа?",
                    "Требуется авторизация", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    NavigationService.Navigate(new LoginPage());
                }
                LvAvailableSlots.SelectedItem = null;
                return;
            }

            if (MainWindow.CurrentUserRole != "Клиент")
            {
                MessageBox.Show("Только клиенты могут записываться на услуги", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                LvAvailableSlots.SelectedItem = null;
                return;
            }

            var selected = (SlotDisplayInfo)LvAvailableSlots.SelectedItem;
            NavigationService.Navigate(new AppointmentBookingPage(selected));
            LvAvailableSlots.SelectedItem = null;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class SlotDisplayInfo
    {
        public DateTime DateTime { get; set; }
        public string TimeDisplay { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public string MasterName { get; set; }
        public int MasterId { get; set; }
        public decimal Price { get; set; }
    }
}
