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
    public partial class ClientAccountPage : Page
    {
        public ClientAccountPage()
        {
            InitializeComponent();
            LoadAppointments();
            LoadOrders();

            LvAppointments.SelectionChanged += LvAppointments_SelectionChanged;
        }

        private void LoadAppointments()
        {
            var appointments = from a in Core.Context.Appointments
                               join st in Core.Context.ServiceTypes on a.ServiceTypeId equals st.Id
                               join m in Core.Context.Users on a.MasterId equals m.Id
                               where a.ClientId == MainWindow.CurrentUserId
                               orderby a.AppointmentDateTime descending
                               select new
                               {
                                   a.AppointmentDateTime,
                                   ServiceName = st.Name,
                                   MasterName = m.FullName,
                                   a.Price,
                                   a.Status
                               };

            var result = appointments.ToList();
            LvAppointments.ItemsSource = result;
        }

        private void LoadOrders()
        {
            var orders = Core.Context.Orders
                .Where(o => o.UserId == MainWindow.CurrentUserId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            LvOrders.ItemsSource = orders;
        }

        private void LvAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvAppointments.SelectedItem != null)
            {
                var selected = LvAppointments.SelectedItem;

                var appointmentDateTime = selected.GetType().GetProperty("AppointmentDateTime")?.GetValue(selected);
                var serviceName = selected.GetType().GetProperty("ServiceName")?.GetValue(selected);
                var masterName = selected.GetType().GetProperty("MasterName")?.GetValue(selected);
                var price = selected.GetType().GetProperty("Price")?.GetValue(selected);
                var status = selected.GetType().GetProperty("Status")?.GetValue(selected);

                MessageBox.Show($"═══════════════════════════════\n" +
                                $"ЗАПИСЬ НА УСЛУГУ\n" +
                                $"═══════════════════════════════\n" +
                                $"Дата и время: {appointmentDateTime:dd.MM.yyyy HH:mm}\n" +
                                $"Услуга: {serviceName}\n" +
                                $"Мастер: {masterName}\n" +
                                $"Цена: {price} руб\n" +
                                $"Статус: {GetAppointmentStatusText(status?.ToString())}\n" +
                                $"═══════════════════════════════",
                                "Детали записи",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                LvAppointments.SelectedItem = null;
            }
        }

        private void LvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvOrders.SelectedItem != null)
            {
                var order = (Orders)LvOrders.SelectedItem;

                var items = (from oi in Core.Context.OrderItems
                             join p in Core.Context.Products on oi.ProductId equals p.Id
                             where oi.OrderId == order.Id
                             select new
                             {
                                 p.Name,
                                 oi.Quantity,
                                 oi.Price,
                                 Total = oi.Quantity * oi.Price
                             }).ToList();

                var itemsList = string.Join("\n", items.Select(i => $"{i.Name} x {i.Quantity} = {i.Total} руб"));

                MessageBox.Show($"═══════════════════════════════\n" +
                                $"ЗАКАЗ №{order.Id}\n" +
                                $"═══════════════════════════════\n" +
                                $"Дата заказа: {order.OrderDate:dd.MM.yyyy}\n" +
                                $"Дата доставки: {order.DeliveryDate:dd.MM.yyyy}\n" +
                                $"Способ оплаты: {order.PaymentMethod ?? "Не указан"}\n" +
                                $"Статус: {GetOrderStatusText(order.Status)}\n" +
                                $"───────────────────────────────\n" +
                                $"Товары:\n{itemsList}\n" +
                                $"───────────────────────────────\n" +
                                $"ИТОГО: {order.TotalAmount} руб\n" +
                                $"═══════════════════════════════",
                                $"Заказ №{order.Id}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                LvOrders.SelectedItem = null;
            }
        }

        private string GetAppointmentStatusText(string status)
        {
            switch (status)
            {
                case "Scheduled": return "Запланирована";
                case "Completed": return "Выполнена";
                case "Cancelled": return "Отменена";
                default: return status;
            }
        }

        private string GetOrderStatusText(string status)
        {
            switch (status)
            {
                case "Pending": return "Ожидает выдачи";
                case "Completed": return "Выдан";
                case "Cancelled": return "Отменен";
                default: return status;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
