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
    public partial class Step2Page : Page
    {
        private MainWindow mainWindow;

        public Step2Page(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            
            if (mainWindow.Configuration.SelectedColor != null)
            {
                foreach (RadioButton rb in colorPanel.Children)
                {
                    if (rb.Tag.ToString() == mainWindow.Configuration.SelectedColor)
                    {
                        rb.IsChecked = true;
                        break;
                    }
                }
            }

            
            foreach (CheckBox cb in optionsPanel.Children)
            {
                string option = cb.Tag.ToString();
                cb.IsChecked = mainWindow.Configuration.SelectedOptions.Contains(option);
            }

            UpdatePrice();
        }

        private void UpdatePrice()
        {
            totalPriceText.Text = $"{mainWindow.Configuration.TotalPrice:N0} ₽";
            basePriceText.Text = $"База: {mainWindow.Configuration.BasePrice:N0} ₽";
            enginePriceText.Text = $"Двигатель: {mainWindow.Configuration.EnginePrice:N0} ₽";
            colorPriceText.Text = $"Цвет: {mainWindow.Configuration.ColorPrice:N0} ₽";
            optionsPriceText.Text = $"Опции: {mainWindow.Configuration.OptionsPrice:N0} ₽";
        }

        private void ColorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                mainWindow.Configuration.SelectedColor = radioButton.Tag.ToString();
                UpdatePrice();
            }
        }

        private void OptionCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                string option = checkBox.Tag.ToString();

                if (checkBox.IsChecked == true)
                {
                    if (!mainWindow.Configuration.SelectedOptions.Contains(option))
                        mainWindow.Configuration.SelectedOptions.Add(option);
                }
                else
                {
                    mainWindow.Configuration.SelectedOptions.Remove(option);
                }

                UpdatePrice();
            }
        }
    }
}
