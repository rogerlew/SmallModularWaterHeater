using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for SliderControl.xaml
    /// </summary>
    public partial class SliderControl : UserControl
    {
        // LowAlarmStr <string>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
                                        typeof(SliderControl),
                                        new PropertyMetadata("Title"));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Minimum <double>
        public static DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double),
                                        typeof(SliderControl),
                                        new PropertyMetadata((object)0.0));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, (double)value); }
        }

        // Maximum <double>
        public static DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double),
                                        typeof(SliderControl),
                                        new PropertyMetadata((object)100.0));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, (double)value); }
        }


        // Value <double>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double),
                                        typeof(SliderControl),
                                        new PropertyMetadata((object)100.0));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, (double)value); }
        }

        public SliderControl()
        {
            InitializeComponent();
        }
    }
}
