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
    public partial class AppointmentBookingPage : Page
    {
        private SlotDisplayInfo _selectedSlot;

        public AppointmentBookingPage(SlotDisplayInfo selectedSlot)
        {
            InitializeComponent();
            _selectedSlot = selectedSlot;
            LoadData();
        }

        private void LoadData()
        {
            TxtService.Text = _selectedSlot.ServiceName;
            TxtMaster.Text = _selectedSlot.MasterName;
            TxtDateTime.Text = _selectedSlot.TimeDisplay;
            TxtPrice.Text = $"{_selectedSlot.Price} руб";
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var confirmResult = MessageBox.Show($"Подтвердите запись:\n\n" +
                $"Мастер: {_selectedSlot.MasterName}\n" +
                $"Услуга: {_selectedSlot.ServiceName}\n" +
                $"Дата и время: {_selectedSlot.TimeDisplay}\n" +
                $"Цена: {_selectedSlot.Price} руб\n\n" +
                $"Продолжить?",
                "Подтверждение записи", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmResult != MessageBoxResult.Yes) return;

            var appointment = new Appointments
            {
                ClientId = MainWindow.CurrentUserId,
                MasterId = _selectedSlot.MasterId,
                ServiceTypeId = _selectedSlot.ServiceId,
                AppointmentDateTime = _selectedSlot.DateTime,
                Price = _selectedSlot.Price,
                PaymentMethod = (CmbPaymentMethod.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Comment = TxtComment.Text,
                Status = "Scheduled",
                CreatedAt = DateTime.Now
            };

            Core.Context.Appointments.Add(appointment);
            Core.Context.SaveChanges();

            MessageBox.Show("Вы успешно записаны!");
            NavigationService.Navigate(new AppointmentsCatalogPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
