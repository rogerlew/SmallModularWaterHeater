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
    /// Interaction logic for TurbineSpeedBarIndicator.xaml
    /// </summary>
    public partial class TurbineSpeedBarIndicator : UserControl
    {
        public double TurbineSpeed;

        private SolidColorBrush SpecialGray = 
            new SolidColorBrush(Color.FromRgb(0xBB, 0xBB, 0xBB));

        // Text <string>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                                        typeof(TurbineSpeedBarIndicator));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public TurbineSpeedBarIndicator()
        {
            TurbineSpeed = 0;
            InitializeComponent();
        }

        public void Update()
        {
            TurbineSpeed = Convert.ToDouble(Text);
            double width = Container.ActualWidth;
            bar.Width = TurbineSpeed / 2000.0 * width;


            if (TurbineSpeed >= 800.0 && TurbineSpeed <= 1000.0)
            {
                resonancebar1.Fill = Brushes.Red;
                resonancebar2.Fill =SpecialGray;
                resonancebar3.Fill =SpecialGray;
                resonancebar4.Fill =SpecialGray;
              
            }
            else if (TurbineSpeed >= 1000.0 && TurbineSpeed <= 1200.0)
            {
                resonancebar1.Fill =SpecialGray;
                resonancebar2.Fill = Brushes.Red;
                resonancebar3.Fill =SpecialGray;
                resonancebar4.Fill =SpecialGray;
               
            }
            else if (TurbineSpeed >= 1200.0 && TurbineSpeed <= 1400.0)
            {
                resonancebar1.Fill =SpecialGray;
                resonancebar2.Fill =SpecialGray;
                resonancebar3.Fill = Brushes.Red;
                resonancebar4.Fill =SpecialGray;
               
            }
            else if (TurbineSpeed >= 1550.0 && TurbineSpeed <= 1750.0)
            {
                resonancebar1.Fill =SpecialGray;
                resonancebar2.Fill =SpecialGray;
                resonancebar3.Fill =SpecialGray;
                resonancebar4.Fill = Brushes.Red;
               
            }
        
            else
            {
                resonancebar1.Fill =SpecialGray;
                resonancebar2.Fill =SpecialGray;
                resonancebar3.Fill =SpecialGray;
                resonancebar4.Fill =SpecialGray;
               
            }
        }
    }
}
