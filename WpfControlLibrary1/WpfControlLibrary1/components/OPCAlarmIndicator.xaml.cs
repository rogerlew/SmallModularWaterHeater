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
    /// Interaction logic for OPCAlarmIndicator.xaml
    /// </summary>
    public partial class OPCAlarmIndicator : UserControl
    {
        public double Status;
        
        // Title <string>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
                                        typeof(OPCAlarmIndicator));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Text <string>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                                        typeof(OPCAlarmIndicator));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public OPCAlarmIndicator()
        {
            Status = 0;
            InitializeComponent();

            Text = "0.0";
        }

        public void Update()
        {
            Status = Convert.ToDouble(Text);

            if (Status == 0)
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0xF3, 0xF3, 0xF3));
            }
            else
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00));
            }
        }
    }
}
