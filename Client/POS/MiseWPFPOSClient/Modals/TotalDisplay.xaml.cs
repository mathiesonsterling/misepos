using System;
using System.Windows;
using System.Threading;

namespace MiseWPFPOSClient.Modals
{
    /// <summary>
    /// Interaction logic for TotalDisplay.xaml
    /// </summary>
    public partial class TotalDisplay : Window
    {
        private decimal _total;
        public TotalDisplay(decimal total)
        {
            InitializeComponent();
            _total = total;
            txtTotal.Text = "$" + _total.ToString("f2");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //after 1 minute, exit
            Thread.Sleep(new TimeSpan(12000000));
            this.Close();
        }

    }
}
