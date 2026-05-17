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
using System.Windows.Shapes;

namespace WpfApp1.Other
{
    public partial class ComplaintWindow : Window
    {
        public string Reason { get; private set; }
        public bool IsSent { get; private set; }

        public ComplaintWindow(string title)
        {
            InitializeComponent();
            TitleText.Text = title;
            IsSent = false;
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReasonTextBox.Text))
            {
                MessageBox.Show("Введите причину жалобы");
                return;
            }

            if (ReasonTextBox.Text.Length < 10)
            {
                MessageBox.Show("Причина жалобы должна содержать минимум 10 символов");
                return;
            }

            Reason = ReasonTextBox.Text;
            IsSent = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            IsSent = false;
            Close();
        }
    }
}
