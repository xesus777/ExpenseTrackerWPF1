using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class Step5Page : Page
    {
        private MainWindow mainWindow;

        public Step5Page(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;
            Loaded += Step5Page_Loaded;
        }

        private void Step5Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            txtName.Text = mainWindow.Configuration.CustomerName ?? "";
            txtPhone.Text = mainWindow.Configuration.Phone ?? "";
            txtEmail.Text = mainWindow.Configuration.Email ?? "";

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            summaryModel.Text = mainWindow.Configuration.SelectedModel ?? "не выбрана";
            summaryEngine.Text = mainWindow.Configuration.SelectedEngine ?? "не выбран";
            summaryColor.Text = mainWindow.Configuration.SelectedColor ?? "не выбран";
            summaryTotalPrice.Text = $"{mainWindow.Configuration.TotalPrice:N0} ₽";
            summaryDownPayment.Text = $"Первоначальный взнос: {mainWindow.Configuration.DownPaymentPercent}%";
            summaryLoanTerm.Text = $"Срок: {mainWindow.Configuration.LoanTerm} месяцев";
            summaryMonthlyPayment.Text = $"Ежемесячный платеж: {mainWindow.Configuration.MonthlyPayment:N0} ₽";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (textBox.Name == "txtName")
            {
                mainWindow.Configuration.CustomerName = textBox.Text;
            }
            else if (textBox.Name == "txtPhone")
            {
                mainWindow.Configuration.Phone = textBox.Text;
            }
            else if (textBox.Name == "txtEmail")
            {
                mainWindow.Configuration.Email = textBox.Text;
            }
        }

        public bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(mainWindow.Configuration.CustomerName) ||
                mainWindow.Configuration.CustomerName.Length < 2)
            {
                MessageBox.Show("Введите корректное имя (минимум 2 символа)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(mainWindow.Configuration.Phone) ||
                !Regex.IsMatch(mainWindow.Configuration.Phone, @"^\d+$"))
            {
                MessageBox.Show("Введите корректный номер телефона (только цифры)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(mainWindow.Configuration.Email) ||
                !IsValidEmail(mainWindow.Configuration.Email))
            {
                MessageBox.Show("Введите корректный email адрес", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            { 
                return false;
            }
        }
    }
}
