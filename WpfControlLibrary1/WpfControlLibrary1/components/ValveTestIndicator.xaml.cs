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

namespace WpfControlLibrary1
{
    /// <summary>
    /// Interaction logic for ValveTestIndicator.xaml
    /// </summary>
    public partial class ValveTestIndicator : UserControl
    {
        public double ValvePosition;

        // Text <string>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                                        typeof(ValveTestIndicator));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ValveTestIndicator()
        {
            ValvePosition = 0;
            InitializeComponent();

            Text = "0.0";
        }

        public void Update(String Text)
        {
            this.Text = Text;
            Update();
        }

        public void Update()
        {
            ValvePosition = Convert.ToDouble(Text);
            ValvePosition = Math.Round(ValvePosition, 3);

            if (ValvePosition == 1)
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                label.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                label.Content = "Open";
            }
            else if (ValvePosition == 0)
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                label.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                label.Content = "Closed";
            }
            else
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0x7F, 0x7F, 0x7F));
                label.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                label.Content = "Open";
            }
        }
    }
}
