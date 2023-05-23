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
    /// Interaction logic for MiniTrend.xaml
    /// </summary>
    public partial class MiniTrend : UserControl
    {

        // Data stores the trend values is list of ints that get mapped 
        // directly to the Grid Container Coordinates
        public int[] Data;

        // n is used to keep track of the last updated index of Data
        // gets reset at 199 so it doesn't need to be a double
        public int n; 

        // constructor
        public MiniTrend()
        {
            InitializeComponent();
            
            // Initialize Data array to container's height
            var H = (int)this.Container.ActualHeight;
            n = 0;
            Data = new int[200];
            for (int i = 0; i < 200; i++)
            {
                Data[i] = H;
            }
        }

        #region DependencyProperty Declarations
        // Title <string>
        public static DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(string), typeof(MiniTrend));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Units <string>
        public static DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(string), typeof(MiniTrend));

        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        // ValueStr <string>
        public static DependencyProperty ValueStrProperty =
            DependencyProperty.Register("ValueStr", typeof(string), typeof(MiniTrend));

        public string ValueStr
        {
            get { return (string)GetValue(ValueStrProperty); }
            set { SetValue(ValueStrProperty, value); }
        }

        // YminStr <string>
        public static DependencyProperty YminStrProperty =
            DependencyProperty.Register("YminStr", typeof(string), typeof(MiniTrend));

        public string YminStr
        {
            get { return (string)GetValue(YminStrProperty); }
            set { SetValue(YminStrProperty, value); }
        }

        // YmaxStr <string>
        public static DependencyProperty YmaxStrProperty =
            DependencyProperty.Register("YmaxStr", typeof(string), typeof(MiniTrend));

        public string YmaxStr
        {
            get { return (string)GetValue(YmaxStrProperty); }
            set { SetValue(YmaxStrProperty, value); }
        }

        // LowAlarmStr <string>
        public static DependencyProperty LowAlarmStrProperty =
            DependencyProperty.Register("LowAlarmStr", typeof(string), typeof(MiniTrend));

        public string LowAlarmStr
        {
            get { return (string)GetValue(LowAlarmStrProperty); }
            set { SetValue(LowAlarmStrProperty, value); }
        }

        // HighAlarmStr <string>
        public static DependencyProperty HighAlarmStrProperty =
            DependencyProperty.Register("HighAlarmStr", typeof(string), typeof(MiniTrend));

        public string HighAlarmStr
        {
            get { return (string)GetValue(HighAlarmStrProperty); }
            set { SetValue(HighAlarmStrProperty, value); }
        }

        // Ymin <double>
        public static DependencyProperty YminProperty = 
            DependencyProperty.Register("Ymin", typeof(double), typeof(MiniTrend),
                                        new PropertyMetadata((object)0.0));

        public double Ymin
        {
            get { return (double)GetValue(YminProperty); }
            set { SetValue(YminProperty, (double)value); }
        }

        // Ymax <double>
        public static DependencyProperty YmaxProperty =
            DependencyProperty.Register("Ymax", typeof(double), typeof(MiniTrend),
                                        new PropertyMetadata((object)100.0));

        public double Ymax
        {
            get { return (double)GetValue(YmaxProperty); }
            set { SetValue(YmaxProperty, (double)value); }
        }

        // LowAlarmSetpoint <double>
        public static DependencyProperty LowAlarmSetpointProperty =
            DependencyProperty.Register("LowAlarmSetpoint", typeof(double), typeof(MiniTrend),
                                        new PropertyMetadata((object)0.0));

        public double LowAlarmSetpoint
        {
            get { return (double)GetValue(LowAlarmSetpointProperty); }
            set { SetValue(LowAlarmSetpointProperty, (double)value); }
        }

        // LowWarningSetpoint <double>
        public static DependencyProperty LowWarningSetpointProperty =
            DependencyProperty.Register("LowWarningSetpoint", typeof(double), typeof(MiniTrend),
                                        new PropertyMetadata((object)0.0));

        public double LowWarningSetpoint
        {
            get { return (double)GetValue(LowWarningSetpointProperty); }
            set { SetValue(LowWarningSetpointProperty, (double)value); }
        }

        // HighAlarmSetpoint <double>
        public static DependencyProperty HighAlarmSetpointProperty =
            DependencyProperty.Register("HighAlarmSetpoint", typeof(double), typeof(MiniTrend),
                                        new PropertyMetadata((object)100.0));

        public double HighAlarmSetpoint
        {
            get { return (double)GetValue(HighAlarmSetpointProperty); }
            set { SetValue(HighAlarmSetpointProperty, (double)value); }
        }

        //HighWarningSetpoint
        public static DependencyProperty HighWarningSetpointProperty =
            DependencyProperty.Register("HighWarningSetpoint", typeof(double), typeof(MiniTrend),
                                        new PropertyMetadata((object)100.0));

        public double HighWarningSetpoint
        {
            get { return (double)GetValue(HighWarningSetpointProperty); }
            set { SetValue(HighWarningSetpointProperty, (double)value); }
        }
        #endregion

        #region Update
        public void Update(double Value)
        {
            var H = this.Container.ActualHeight;
            var rng = Ymax - Ymin;
            String PathStr = "";


            // update Ylim labels
            YminStr = String.Format("{0:0}", Ymin);
            YmaxStr = String.Format("{0:0}", Ymax);

            // If Alarm Setpoints are specified, Update them and the left ruler
            if (LowAlarmSetpoint > Ymin)
            {
                LowAlarmStr = String.Format("{0:0}", LowAlarmSetpoint);
            }
            if (HighAlarmSetpoint < Ymax)
            {
                HighAlarmStr = String.Format("{0:0}", HighAlarmSetpoint);
            }
            if ((LowAlarmSetpoint > Ymin) && (HighAlarmSetpoint < Ymax))
            {
                double HighPx = H * ((Ymax - HighAlarmSetpoint) / rng);
                double LowPx = H * (1 - (LowAlarmSetpoint - Ymin) / rng);
                this.AlarmLimPath.Data = Geometry.Parse(PathStr);
                PathStr = String.Format("M 170,{0:0.0} 180,{0:0.0} 180,{1:0.0} 170,{1:0.0}", HighPx, LowPx);
            }
            this.AlarmLimPath.Data = Geometry.Parse(PathStr);

            // Update Alarm Fills
            this.LowAlarmRect.Height = (int)Math.Round(H * ((LowAlarmSetpoint - Ymin) / rng));
            this.HighAlarmRect.Height = (int)Math.Round(H * ((Ymax - HighAlarmSetpoint) / rng));
            this.LowWarningRect.Height = (int)Math.Round(H * ((LowWarningSetpoint - LowAlarmSetpoint) / rng));
            this.HighWarningRect.Height = (int)Math.Round(H * ((HighAlarmSetpoint - HighWarningSetpoint) / rng));
            this.LowWarningRect.Margin = new Thickness(0, 0, 0, this.LowAlarmRect.Height);
            this.HighWarningRect.Margin = new Thickness(0, this.HighAlarmRect.Height, 0, 0);

            if (Ymin < Value && Value < LowAlarmSetpoint)
            {
                LowAlarmRect.Opacity = 1.0;
                LowWarningRect.Opacity = 0.2;
                HighWarningRect.Opacity = 0.2;
                HighAlarmRect.Opacity = 0.2;
            }
            else if (LowAlarmSetpoint < Value && Value < LowWarningSetpoint)
            {
                LowAlarmRect.Opacity = 0.2;
                LowWarningRect.Opacity = 1.0;
                HighWarningRect.Opacity = 0.2;
                HighAlarmRect.Opacity = 0.2;
            }
            else if (HighWarningSetpoint < Value && Value < HighAlarmSetpoint)
            {
                LowAlarmRect.Opacity = 0.2;
                LowWarningRect.Opacity = 0.2;
                HighWarningRect.Opacity = 1.0;
                HighAlarmRect.Opacity = 0.2;
            }
            else if (HighWarningSetpoint < Value && Value < Ymax)
            {
                LowAlarmRect.Opacity = 0.2;
                LowWarningRect.Opacity = 0.2;
                HighWarningRect.Opacity = 0.2;
                HighAlarmRect.Opacity = 1.0;
            }
            else
            {
                LowAlarmRect.Opacity = 0.2;
                LowWarningRect.Opacity = 0.2;
                HighWarningRect.Opacity = 0.2;
                HighAlarmRect.Opacity = 0.2;
            }

            // update the live trend with the new value
            ValueStr = String.Format("{0:0.0}", Value);

            int index = ((n % 200) + 1) % 200;
            Data[index] = (int)(H - Math.Round(H * ((Value - Ymin) / rng), 0));

            // Rebuild the path
            // Might be a better way to do this without having to 
            // build the path string first
            PathStr = "M";
            for (int i = 1; i < 200; i++)
            {
                index = ((n % 200) + 1 + i) % 200;
                PathStr += String.Format(" {0:0},{1:0}", i, Data[index]);
            }

            this.Trend.Data = Geometry.Parse(PathStr);

            // update the counter
            n++;
            n %= 200;
        }
        #endregion

    }
}
