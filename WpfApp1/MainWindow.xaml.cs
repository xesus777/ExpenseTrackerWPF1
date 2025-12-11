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
using WpfApp1.Models;
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public CarConfiguration Configuration { get; set; }
        private int currentStep = 1;

        public MainWindow()
        {
            InitializeComponent();
            Configuration = new CarConfiguration();
            LoadStep(1);
        }

        private void LoadStep(int step)
        {
            currentStep = step;
            progressBar.Value = step;
            stepText.Text = $"{step} из 5";

            switch (step)
            {
                case 1:
                    mainFrame.Navigate(new Step1Page(this));
                    break;
                case 2:
                    mainFrame.Navigate(new Step2Page(this));
                    break;
                case 3:
                    mainFrame.Navigate(new Step3Page(this));
                    break;
                case 4:
                    mainFrame.Navigate(new Step4Page(this));
                    break;
                case 5:
                    mainFrame.Navigate(new Step5Page(this));
                    break;
            }

            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            btnBack.Visibility = currentStep > 1 ? Visibility.Visible : Visibility.Collapsed;
            btnNext.Content = currentStep == 5 ? "Оформить заявку" : "Далее";
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep > 1)
            {
                LoadStep(currentStep - 1);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep < 5)
            {
                LoadStep(currentStep + 1);
            }
            else
            {
                
                var currentPage = mainFrame.Content as Step5Page;
                if (currentPage != null && currentPage.ValidateData())
                {
                    MessageBox.Show("Заявка успешно оформлена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
            }
        }

        public void NavigateToStep(int step)
        {
            LoadStep(step);
        }
    }
}
