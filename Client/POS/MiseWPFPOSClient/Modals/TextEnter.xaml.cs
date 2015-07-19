using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MiseWPFPOSClient.Modals
{

    /// <summary>
    /// Interaction logic for TextEnter.xaml
    /// </summary>
    public partial class TextEnter
    {
        private readonly List<string> _letters1 = new List<string> {"Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P"};
        private readonly List<string> _letters2 = new List<string> {"A", "S", "D", "F", "G", "H", "J", "K", "L"};
        private readonly List<string> _letters3 = new List<string> {"Z", "X", "C", "V", "B", "N", "M"};

    public string EnteredText
        {
            get
            {
                if (txtRes != null)
                {
                    return txtRes.Text;
                }

                return string.Empty;
            }
        }

        public TextEnter()
        {
            InitializeComponent();
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            txtRes.Text = string.Empty;
            Close();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            icKeyboardRow1.ItemsSource = _letters1;
            icKeyboardRow2.ItemsSource = _letters2;
            icKeyboardRow3.ItemsSource = _letters3;
        }

        private void LetterClicked(object sender, RoutedEventArgs e)
        {
            var btn = (Button) sender;
            txtRes.Text += btn.CommandParameter.ToString();
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            if(EnteredText.Length > 0)
            {
                txtRes.Text = txtRes.Text.Substring(0, txtRes.Text.Length - 1);
            }
        }

        private void EnterClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
