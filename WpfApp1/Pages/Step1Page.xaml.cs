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
    public partial class Step1Page : Page
    {
        private MainWindow mainWindow;

        public Step1Page(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (mainWindow.Configuration.SelectedModel != null)
            {
                foreach (RadioButton rb in modelPanel.Children)
                {
                    if (rb.Tag.ToString() == mainWindow.Configuration.SelectedModel)
                    {
                        rb.IsChecked = true;
                        break;
                    }
                }
            }

            if (mainWindow.Configuration.SelectedEngine != null)
            {
                foreach (RadioButton rb in enginePanel.Children)
                {
                    if (rb.Tag.ToString() == mainWindow.Configuration.SelectedEngine)
                    {
                        rb.IsChecked = true;
                        break;
                    }
                }
            }

            UpdatePrice();
        }

        private void UpdatePrice()
        {
            totalPriceText.Text = $"{mainWindow.Configuration.TotalPrice:N0} ₽";
            basePriceText.Text = $"Базовая цена: {mainWindow.Configuration.BasePrice:N0} ₽";
            enginePriceText.Text = $"Двигатель: +{mainWindow.Configuration.EnginePrice:N0} ₽";
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null) return;

            if (radioButton.GroupName == "Model")
            {
                mainWindow.Configuration.SelectedModel = radioButton.Tag.ToString();
            }
            else if (radioButton.GroupName == "Engine")
            {
                mainWindow.Configuration.SelectedEngine = radioButton.Tag.ToString();
            }

            UpdatePrice();
        }
    }
}
