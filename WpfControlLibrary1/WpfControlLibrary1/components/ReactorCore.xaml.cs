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
    /// Interaction logic for ReactorCore.xaml
    /// </summary>
    public partial class ReactorCore : UserControl
    {
        public ReactorCore()
        {
			this.InitializeComponent();
        }
        
        // Angle <double>
        public static DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double),
                                        typeof(ReactorCore),
                                        new PropertyMetadata((object)0.0));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, (double)value); }
        }

        // Text <string>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                                        typeof(ReactorCore));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public void Update()
        {
            Angle = Convert.ToDouble(Text) * 3.0;
            Needle.RenderTransform = new RotateTransform(Angle);
        }
    }
}