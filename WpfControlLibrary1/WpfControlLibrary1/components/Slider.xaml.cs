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
    /// Interaction logic for Slider.xaml
    /// </summary>
    public partial class Slider : UserControl
    {

        public Slider()
        {
            InitializeComponent();
            
        }

        #region DependencyProperty Declarations
    

        // ValueStr <string>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string),
                                        typeof(Slider));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value);
            }
        }

        // ValueStr <string>
        public static DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(string),
                                        typeof(Slider));

        public string MinValue
        {
            get { return (string)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // ValueStr <string>
        public static DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(string),
                                        typeof(Slider));

        public string MaxValue
        {
            get { return (string)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // ValueStr <string>
        public static DependencyProperty StepValueProperty =
            DependencyProperty.Register("StepValue", typeof(string),
                                        typeof(Slider));

        public string StepValue
        {
            get { return (string)GetValue(StepValueProperty); }
            set { SetValue(StepValueProperty, value); }
        }
        // ValueStr <string>
        public static DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(string),
                                        typeof(Slider));

        public string Range
        {
            get { return (string)GetValue(RangeProperty); }
            set { SetValue(RangeProperty, value);
            }
        }

        #endregion
        private void PathMover(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                Point Location = Mouse.GetPosition(this);
                if (Location.X > 19 && Location.X < 221)
                {
                    StringBuilder newpoint = new StringBuilder();
                    newpoint.Append("M ");
                    newpoint.Append(Location.X);
                    newpoint.Append(",25 ");
                    newpoint.Append(Location.X);
                    newpoint.Append(",5");

                    Position.Data = Geometry.Parse(newpoint.ToString());
                    Value = ((Location.X - 20) / 200 * (int.Parse(Range))).ToString("0");
               
                }
            }
        }
        private void PathMover1(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                Point Location = Mouse.GetPosition(this);
                if (Location.X > 19 && Location.X < 221)
                {
                    if ((Location.X - 20) / 200 * (int.Parse(Range)) >= (int.Parse(Value) + int.Parse(StepValue)))
                    {
                        Location = new Point((int.Parse(Value) + int.Parse(StepValue)) * 200 / (int.Parse(Range))+20, 0);
                    }
                    else if ((Location.X - 20) / 200 * (int.Parse(Range)) <= (int.Parse(Value) - int.Parse(StepValue)))
                    {
                        Location = new Point((int.Parse(Value) - int.Parse(StepValue)) * 200 / (int.Parse(Range))+20, 0);
                    }
                    StringBuilder newpoint = new StringBuilder();
                    newpoint.Append("M ");
                    newpoint.Append(Location.X);
                    newpoint.Append(",25 ");
                    newpoint.Append(Location.X);
                    newpoint.Append(",5");

                    Position.Data = Geometry.Parse(newpoint.ToString());
                    Value = ((Location.X - 20) / 200 * (int.Parse(Range))).ToString("0");
                   
                    
                }
            }
        }


        public void Update()
        {

            Point Location = new Point((int.Parse(Value) * 200 / (int.Parse(Range))) + 20, 0);
            StringBuilder newpoint = new StringBuilder();
            newpoint.Append("M ");
            newpoint.Append(Location.X);
            newpoint.Append(",25 ");
            newpoint.Append(Location.X);
            newpoint.Append(",5");
            Position.Data = Geometry.Parse(newpoint.ToString());
            Value = ((Location.X - 20) / 200 * (int.Parse(Range))).ToString("0");

        }

       
    }
}
