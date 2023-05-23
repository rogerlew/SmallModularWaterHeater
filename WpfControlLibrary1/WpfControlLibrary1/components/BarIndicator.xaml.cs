using System;
using System.Diagnostics;
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
    /// Interaction logic for BarIndicator.xaml
    /// </summary>
    public partial class BarIndicator : UserControl
    {

        // n is used to keep track of the last updated index of Data
        // gets reset at 199 so it doesn't need to be a double
        public int n;

        // Data stores the trend values is list of ints that get mapped 
        // directly to the Grid Container Coordinates
        public int[] Data;
        public double[] CIData;
        public double[] SensorNoise;
        int noiseN;
        Random random;


        public BarIndicator()
        {
            InitializeComponent();

            random = new Random();

            // Initialize Data array to container's height
            var H = (int)this.Container.ActualHeight;
            n = 0;
            Data = new int[200];
            CIData = new double[200];
            for (int i = 0; i < 200; i++)
            {
                Data[i] = H;
                CIData[i] = 0;
            }

            // Initialize array for averaging sensor noise
            noiseN = 16;
            SensorNoise = new double[noiseN];
            for (int i = 0; i < noiseN; i++)
            {
                SensorNoise[i] = Normal(NoiseStd);
            }

        }

        #region DependencyProperty Declarations
        // Title <string>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(BarIndicator));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Units <string>
        public static DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(string), typeof(BarIndicator));

        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        // ValueStr <string>
        public static DependencyProperty ValueStrProperty =
            DependencyProperty.Register("ValueStr", typeof(string), typeof(BarIndicator));

        public string ValueStr
        {
            get { return (string)GetValue(ValueStrProperty); }
            set { SetValue(ValueStrProperty, value); }
        }

        // YminStr <string>
        public static DependencyProperty YminStrProperty =
            DependencyProperty.Register("YminStr", typeof(string), typeof(BarIndicator));

        public string YminStr
        {
            get { return (string)GetValue(YminStrProperty); }
            set { SetValue(YminStrProperty, value); }
        }

        // YmaxStr <string>
        public static DependencyProperty YmaxStrProperty =
            DependencyProperty.Register("YmaxStr", typeof(string), typeof(BarIndicator));

        public string YmaxStr
        {
            get { return (string)GetValue(YmaxStrProperty); }
            set { SetValue(YmaxStrProperty, value); }
        }

        // LowAlarmStr <string>
        public static DependencyProperty LowAlarmStrProperty =
            DependencyProperty.Register("LowAlarmStr", typeof(string), typeof(BarIndicator));

        public string LowAlarmStr
        {
            get { return (string)GetValue(LowAlarmStrProperty); }
            set { SetValue(LowAlarmStrProperty, value); }
        }

        // HighAlarmStr <string>
        public static DependencyProperty HighAlarmStrProperty =
            DependencyProperty.Register("HighAlarmStr", typeof(string), typeof(BarIndicator));

        public string HighAlarmStr
        {
            get { return (string)GetValue(HighAlarmStrProperty); }
            set { SetValue(HighAlarmStrProperty, value); }
        }
        // NoiseStd <double>
        public static DependencyProperty NoiseStdProperty =
            DependencyProperty.Register("NoiseStd", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)0.0));

        public double NoiseStd
        {
            get { return (double)GetValue(NoiseStdProperty); }
            set { SetValue(NoiseStdProperty, (double)value); }
        }

        // NumSensors <double>
        public static DependencyProperty NumSensorsProperty =
            DependencyProperty.Register("NumSensors", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)1.0));

        public double NumSensors
        {
            get { return (double)GetValue(NumSensorsProperty); }
            set { SetValue(NumSensorsProperty, (double)value); }
        }

        // NoisyValue <double>
        public static DependencyProperty NoisyValueProperty =
            DependencyProperty.Register("NoisyValue", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)0.0));

        public double NoisyValue
        {
            get { return (double)GetValue(NoisyValueProperty); }
            set { SetValue(NoisyValueProperty, (double)value); }
        }

        // Ymin <double>
        public static DependencyProperty YminProperty =
            DependencyProperty.Register("Ymin", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)0.0));

        public double Ymin
        {
            get { return (double)GetValue(YminProperty); }
            set { SetValue(YminProperty, (double)value); }
        }

        // Ymax <double>
        public static DependencyProperty YmaxProperty =
            DependencyProperty.Register("Ymax", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)100.0));

        public double Ymax
        {
            get { return (double)GetValue(YmaxProperty); }
            set { SetValue(YmaxProperty, (double)value); }
        }

        // LowAlarmSetpoint <double>
        public static DependencyProperty LowAlarmSetpointProperty =
            DependencyProperty.Register("LowAlarmSetpoint", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)0.0));

        public double LowAlarmSetpoint
        {
            get { return (double)GetValue(LowAlarmSetpointProperty); }
            set { SetValue(LowAlarmSetpointProperty, (double)value); }
        }

        // LowWarningSetpoint <double>
        public static DependencyProperty LowWarningSetpointProperty =
            DependencyProperty.Register("LowWarningSetpoint", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)0.0));

        public double LowWarningSetpoint
        {
            get { return (double)GetValue(LowWarningSetpointProperty); }
            set { SetValue(LowWarningSetpointProperty, (double)value); }
        }

        // HighAlarmSetpoint <double>
        public static DependencyProperty HighAlarmSetpointProperty =
            DependencyProperty.Register("HighAlarmSetpoint", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)100.0));

        public double HighAlarmSetpoint
        {
            get { return (double)GetValue(HighAlarmSetpointProperty); }
            set { SetValue(HighAlarmSetpointProperty, (double)value); }
        }

        //HighWarningSetpoint
        public static DependencyProperty HighWarningSetpointProperty =
            DependencyProperty.Register("HighWarningSetpoint", typeof(double), typeof(BarIndicator),
                                        new PropertyMetadata((object)100.0));

        public double HighWarningSetpoint
        {
            get { return (double)GetValue(HighWarningSetpointProperty); }
            set { SetValue(HighWarningSetpointProperty, (double)value); }
        }

        #endregion

        #region Support Functions
        private double Normal(double stdDev)
        {
            // http://stackoverflow.com/questions/218060/random-gaussian-variables

            if (stdDev == 0.0)
            {
                return 0.0;
            }
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2);
            return stdDev * randStdNormal;
        }

        private double SampleStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
        #endregion

        public void Update(double Value)
        {
            double H = this.Container.ActualHeight;
            double rng = Ymax - Ymin;

            // update the live trend with the new value
            int index = ((n % 200) + 1) % 200;

            UpdateStaticElements(Value, H, rng);
            UpdateAlarmStatus(Value, H, rng);
            UpdateNoiseModel(Value, H, rng, index);
            UpdateTrend(Value, H, rng, index);

            if (NumSensors > 1)
            {
                UpdateCIFill(Value, H, rng, index);
            }
            else
            {
                this.CI.Opacity = 0.0;
                this.CI2.Opacity = 0.0;
            }

            // update the counter
            n++;
            n %= 200;
        }

        private void UpdateStaticElements(double Value, double H, double rng)
        {
            // update Ylim labels
            YminStr = String.Format("{0:0}", Ymin);
            YmaxStr = String.Format("{0:0}", Ymax);

            // If Alarm Setpoints are specified, Update them and the left ruler

            double HighPx = H * ((Ymax - HighAlarmSetpoint) / rng);
            double LowPx = H * (1 - (LowAlarmSetpoint - Ymin) / rng);

            if (LowAlarmSetpoint > Ymin)
            {
                LowAlarmStr = String.Format("{0:0}", LowAlarmSetpoint);
                LowAlarmLabel.Margin = new Thickness(65.0, LowPx - 16.0, 15.0, (H - LowPx) - 16.0);
            }
            if (HighAlarmSetpoint < Ymax)
            {
                HighAlarmStr = String.Format("{0:0}", HighAlarmSetpoint);
                HighAlarmLabel.Margin = new Thickness(65.0, HighPx - 16.0, 15.0, (H - HighPx) - 16.0);
            }

            String PathStr = "";
            if ((LowAlarmSetpoint > Ymin) && (HighAlarmSetpoint < Ymax))
            {
                this.AlarmLimPath.Data = Geometry.Parse(PathStr);
                PathStr = String.Format("M 170,{0:0.0} 180,{0:0.0} 180,{1:0.0} 170,{1:0.0}", HighPx, LowPx);
            }
            this.AlarmLimPath.Data = Geometry.Parse(PathStr);
        }

        private void UpdateAlarmStatus(double Value, double H, double rng)
        {
            // Update Alarm Fill
            if (Ymin < Value && Value <= LowAlarmSetpoint)
            {
                this.AlarmRect.Background = Brushes.Red;
                this.AlarmRect.Opacity = 0.8;
            }
            else if (LowAlarmSetpoint < Value && Value <= LowWarningSetpoint)
            {
                this.AlarmRect.Background = Brushes.Yellow;
                this.AlarmRect.Opacity = 0.8;
            }
            else if (HighWarningSetpoint < Value && Value <= HighAlarmSetpoint)
            {
                this.AlarmRect.Background = Brushes.Yellow;
                this.AlarmRect.Opacity = 0.8;
            }
            else if (HighWarningSetpoint < Value && Value <= Ymax)
            {
                this.AlarmRect.Background = Brushes.Red;
                this.AlarmRect.Opacity = 0.8;
            }
            else
            {
                this.AlarmRect.Background = Brushes.LightGray;
                this.AlarmRect.Opacity = 1.0;
            }
        }

        private void UpdateNoiseModel(double Value, double H, double rng, int index)
        {
            // Model Sensor Noise
            double[] SampleNoise = new double[(int)NumSensors];
            for (int i = 0; i < NumSensors; i++)
            {
                SampleNoise[i] = Normal(NoiseStd);
            }

            SensorNoise[n % noiseN] = SampleNoise.Average();
            NoisyValue = Value + SensorNoise.Average();

            double NewValue = (H - H * ((NoisyValue - Ymin) / rng));
            if (NewValue > H)
            {
                NewValue = H;
            }
            if (NewValue < 0)
            {
                NewValue = 0;
            }

            // Save NewValue to List
            Data[index] = (int)Math.Round(NewValue, 0);

            // Save new CI to List
            if (NumSensors > 1.0)
            {
                CIData[index] = (H - SampleStdDev(SampleNoise) / Math.Sqrt(NumSensors) * 1.96);
            }
        }

        private void UpdateTrend(double Value, double H, double rng, int index)
        {
            String PathStr = "";

            // Rebuild the Data path
            // Might be a better way to do this without having to 
            // build the path string first
            PathStr = "M";
            for (int i = 1; i < 200; i++)
            {
                index = ((n % 200) + 1 + i) % 200;
                PathStr += String.Format(" {0:0},{1:0}", i, Data[index]);
            }

            this.Trend.Data = Geometry.Parse(PathStr);
            ValueStr = String.Format("{0:0} ", NoisyValue);
        }

        private void UpdateCIFill(double Value, double H, double rng, int index)
        {
            Debug.Assert(NumSensors > 1.0);

            String PathStr = "M";
            for (int i = 1; i < 200; i++)
            {
                index = ((n % 200) + 1 + i) % 200;
                PathStr += String.Format(" {0:0},{1:0}", i, Data[index] + CIData[index]);
            }
            this.CI.Data = Geometry.Parse(PathStr);

            PathStr = "M";
            for (int i = 1; i < 200; i++)
            {
                index = ((n % 200) + 1 + i) % 200;
                PathStr += String.Format(" {0:0},{1:0}", i, Data[index] - CIData[index]);
            }
            this.CI2.Data = Geometry.Parse(PathStr);
        }
    }
}
