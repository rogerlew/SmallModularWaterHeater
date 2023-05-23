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
    /// Interaction logic for ValveStatusIndicator.xaml
    /// </summary>
    public partial class ValveStatusIndicator : UserControl
    {
        public double ValvePosition;
        
        // Title <string>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
                                        typeof(ValveStatusIndicator));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Text <string>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                                        typeof(ValveStatusIndicator));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ValveStatusIndicator()
        {
            ValvePosition = 0;
            InitializeComponent();

            Text = "0.0";
        }

        public void Update()
        {
            ValvePosition = Convert.ToDouble(Text);
            ValvePosition = Math.Round(ValvePosition, 3);

            if (ValvePosition == 1)
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                label.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            }
            else if (ValvePosition == 0)
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                label.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else
            {
                byte gray = Convert.ToByte((UInt16)(ValvePosition*128.0 + 64.0));
                rect.Fill = new SolidColorBrush(Color.FromRgb(gray, gray, gray));

                if (gray > 0x7F)
                {
                    label.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                }
                else
                {
                    label.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                }
            }
        }
    }
}
