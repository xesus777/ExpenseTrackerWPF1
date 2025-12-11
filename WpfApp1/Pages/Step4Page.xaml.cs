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
    public partial class Step4Page : Page
    {
        private MainWindow mainWindow;

        public Step4Page(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;
            Loaded += Step4Page_Loaded;
        }

        private void Step4Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            downPaymentSlider.Value = mainWindow.Configuration.DownPaymentPercent;
            loanTermSlider.Value = mainWindow.Configuration.LoanTerm;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            
            downPaymentPercentText.Text = $"Первоначальный взнос: {mainWindow.Configuration.DownPaymentPercent}%";
            loanTermText.Text = $"Срок кредита: {mainWindow.Configuration.LoanTerm} месяцев";
            carPriceText.Text = $"Стоимость автомобиля: {mainWindow.Configuration.TotalPrice:N0} ₽";

            
            carPriceCalcText.Text = $"{mainWindow.Configuration.TotalPrice:N0} ₽";
            downPaymentAmountText.Text = $"{mainWindow.Configuration.DownPaymentAmount:N0} ₽";
            loanAmountText.Text = $"{mainWindow.Configuration.LoanAmount:N0} ₽";
            loanTermCalcText.Text = $"{mainWindow.Configuration.LoanTerm} месяцев";
            monthlyPaymentText.Text = $"{mainWindow.Configuration.MonthlyPayment:N0} ₽";
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mainWindow == null) return;

            var slider = sender as Slider;
            if (slider == null) return;

            if (slider.Name == "downPaymentSlider")
            {
                mainWindow.Configuration.DownPaymentPercent = (int)slider.Value;
            }
            else if (slider.Name == "loanTermSlider")
            {
                mainWindow.Configuration.LoanTerm = (int)slider.Value;
            }

            UpdateDisplay();
        }
    }
}
