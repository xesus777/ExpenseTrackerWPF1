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
    public partial class Step3Page : Page
    {
        private MainWindow mainWindow;

        public Step3Page(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;
            Loaded += Step3Page_Loaded;
        }

        private void Step3Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            
            modelText.Text = $"Модель: {mainWindow.Configuration.SelectedModel ?? "не выбрана"}";
            engineText.Text = $"Двигатель: {mainWindow.Configuration.SelectedEngine ?? "не выбран"}";
            colorText.Text = $"Цвет: {mainWindow.Configuration.SelectedColor ?? "не выбран"}";

            
            optionsListPanel.Children.Clear();
            foreach (var option in mainWindow.Configuration.SelectedOptions)
            {
                optionsListPanel.Children.Add(new TextBlock
                {
                    Text = option,
                    Margin = new Thickness(10, 2, 0, 2)
                });
            }

            if (mainWindow.Configuration.SelectedOptions.Count == 0)
            {
                optionsListPanel.Children.Add(new TextBlock
                {
                    Text = "Нет выбранных опций",
                    FontStyle = FontStyles.Italic,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Margin = new Thickness(10, 2, 0, 2)
                });
            }

            
            basePriceText.Text = $"{mainWindow.Configuration.BasePrice:N0} ₽";
            enginePriceText.Text = $"+{mainWindow.Configuration.EnginePrice:N0} ₽";
            colorPriceText.Text = $"+{mainWindow.Configuration.ColorPrice:N0} ₽";
            optionsPriceText.Text = $"+{mainWindow.Configuration.OptionsPrice:N0} ₽";
            totalPriceText.Text = $"{mainWindow.Configuration.TotalPrice:N0} ₽";
        }
    }
}
