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
    /// Interaction logic for TrendDisplay.xaml
    /// </summary>
    public partial class TrendDisplay : UserControl
    {

        // n is used to keep track of the last updated index of Data
        // gets reset at 199 so it doesn't need to be a double
        public int n;

        // Data stores the trend values is list of ints that get mapped 
        // directly to the Grid Container Coordinates
        public double[] Data;
        public double[] CIData;
        public double[] SensorNoise;
        int noiseN;
        Random random;


        public TrendDisplay()
        {
            InitializeComponent();

            random = new Random();

            // Initialize Data array to container's height
            double H = this.Container.ActualHeight;
            n = 0;
            Data = new double[200];
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
            DependencyProperty.Register("Title", typeof(string),
                                        typeof(TrendDisplay));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Units <string>
        public static DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(string),
                                        typeof(TrendDisplay));

        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        // ValueStr <string>
        public static DependencyProperty ValueStrProperty =
            DependencyProperty.Register("ValueStr", typeof(string),
                                        typeof(TrendDisplay));

        public string ValueStr
        {
            get { return (string)GetValue(ValueStrProperty); }
            set { SetValue(ValueStrProperty, value); }
        }

        // YminStr <string>
        public static DependencyProperty YminStrProperty =
            DependencyProperty.Register("YminStr", typeof(string),
                                        typeof(TrendDisplay));

        public string YminStr
        {
            get { return (string)GetValue(YminStrProperty); }
            set { SetValue(YminStrProperty, value); }
        }

        // YmaxStr <string>
        public static DependencyProperty YmaxStrProperty =
            DependencyProperty.Register("YmaxStr", typeof(string),
                                        typeof(TrendDisplay));

        public string YmaxStr
        {
            get { return (string)GetValue(YmaxStrProperty); }
            set { SetValue(YmaxStrProperty, value); }
        }

        // LowAlarmStr <string>
        public static DependencyProperty LowAlarmStrProperty =
            DependencyProperty.Register("LowAlarmStr", typeof(string),
                                        typeof(TrendDisplay));

        public string LowAlarmStr
        {
            get { return (string)GetValue(LowAlarmStrProperty); }
            set { SetValue(LowAlarmStrProperty, value); }
        }

        // LowWarningStr <string>
        public static DependencyProperty LowWarningStrProperty =
            DependencyProperty.Register("LowWarningStr", typeof(string),
                                        typeof(TrendDisplay));

        public string LowWarningStr
        {
            get { return (string)GetValue(LowWarningStrProperty); }
            set { SetValue(LowWarningStrProperty, value); }
        }

        // HighWarningStr <string>
        public static DependencyProperty HighWarningStrProperty =
            DependencyProperty.Register("HighWarningStr", typeof(string),
                                        typeof(TrendDisplay));

        public string HighWarningStr
        {
            get { return (string)GetValue(HighWarningStrProperty); }
            set { SetValue(HighWarningStrProperty, value); }
        }
        // HighAlarmStr <string>
        public static DependencyProperty HighAlarmStrProperty =
            DependencyProperty.Register("HighAlarmStr", typeof(string),
                                        typeof(TrendDisplay));

        public string HighAlarmStr
        {
            get { return (string)GetValue(HighAlarmStrProperty); }
            set { SetValue(HighAlarmStrProperty, value); }
        }

        // LowWarningLabelVerticalOffset <double>
        public static DependencyProperty LowWarningLabelVerticalOffsetProperty =
            DependencyProperty.Register("LowWarningLabelVerticalOffset", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double LowWarningLabelVerticalOffset
        {
            get { return (double)GetValue(LowWarningLabelVerticalOffsetProperty); }
            set { SetValue(LowWarningLabelVerticalOffsetProperty, (double)value); }
        }

        // HighWarningLabelVerticalOffset <double>
        public static DependencyProperty HighWarningLabelVerticalOffsetProperty =
            DependencyProperty.Register("HighWarningLabelVerticalOffset", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double HighWarningLabelVerticalOffset
        {
            get { return (double)GetValue(HighWarningLabelVerticalOffsetProperty); }
            set { SetValue(HighWarningLabelVerticalOffsetProperty, (double)value); }
        }

        // LowAlarmLabelVerticalOffset <double>
        public static DependencyProperty LowAlarmLabelVerticalOffsetProperty =
            DependencyProperty.Register("LowAlarmLabelVerticalOffset", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double LowAlarmLabelVerticalOffset
        {
            get { return (double)GetValue(LowAlarmLabelVerticalOffsetProperty); }
            set { SetValue(LowAlarmLabelVerticalOffsetProperty, (double)value); }
        }

        // HighAlarmLabelVerticalOffset <double>
        public static DependencyProperty HighAlarmLabelVerticalOffsetProperty =
            DependencyProperty.Register("HighAlarmLabelVerticalOffset", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double HighAlarmLabelVerticalOffset
        {
            get { return (double)GetValue(HighAlarmLabelVerticalOffsetProperty); }
            set { SetValue(HighAlarmLabelVerticalOffsetProperty, (double)value); }
        }

        // NoiseStd <double>
        public static DependencyProperty NoiseStdProperty =
            DependencyProperty.Register("NoiseStd", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double NoiseStd
        {
            get { return (double)GetValue(NoiseStdProperty); }
            set { SetValue(NoiseStdProperty, (double)value); }
        }

        // NumSensors <double>
        public static DependencyProperty NumSensorsProperty =
            DependencyProperty.Register("NumSensors", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)1.0));

        public double NumSensors
        {
            get { return (double)GetValue(NumSensorsProperty); }
            set { SetValue(NumSensorsProperty, (double)value); }
        }

        // NoisyValue <double>
        public static DependencyProperty NoisyValueProperty =
            DependencyProperty.Register("NoisyValue", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double NoisyValue
        {
            get { return (double)GetValue(NoisyValueProperty); }
            set { SetValue(NoisyValueProperty, (double)value); }
        }

        // Ymin <double>
        public static DependencyProperty YminProperty =
            DependencyProperty.Register("Ymin", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double Ymin
        {
            get { return (double)GetValue(YminProperty); }
            set { SetValue(YminProperty, (double)value); }
        }

        // Ymax <double>
        public static DependencyProperty YmaxProperty =
            DependencyProperty.Register("Ymax", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)100.0));

        public double Ymax
        {
            get { return (double)GetValue(YmaxProperty); }
            set { SetValue(YmaxProperty, (double)value); }
        }

        // LowAlarmSetpoint <double>
        public static DependencyProperty LowAlarmSetpointProperty =
            DependencyProperty.Register("LowAlarmSetpoint", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double LowAlarmSetpoint
        {
            get { return (double)GetValue(LowAlarmSetpointProperty); }
            set { SetValue(LowAlarmSetpointProperty, (double)value); }
        }

        // LowWarningSetpoint <double>
        public static DependencyProperty LowWarningSetpointProperty =
            DependencyProperty.Register("LowWarningSetpoint", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)0.0));

        public double LowWarningSetpoint
        {
            get { return (double)GetValue(LowWarningSetpointProperty); }
            set { SetValue(LowWarningSetpointProperty, (double)value); }
        }

        // HighAlarmSetpoint <double>
        public static DependencyProperty HighAlarmSetpointProperty =
            DependencyProperty.Register("HighAlarmSetpoint", typeof(double),
                                        typeof(TrendDisplay),
                                        new PropertyMetadata((object)100.0));

        public double HighAlarmSetpoint
        {
            get { return (double)GetValue(HighAlarmSetpointProperty); }
            set { SetValue(HighAlarmSetpointProperty, (double)value); }
        }

        //HighWarningSetpoint
        public static DependencyProperty HighWarningSetpointProperty =
            DependencyProperty.Register("HighWarningSetpoint", typeof(double),
                                        typeof(TrendDisplay),
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
            Update(Value, 0.0);
        }
        public void Update(double Value, double DriftingSensorValue)
        {
            double H = this.Container.ActualHeight;
            double rng = Ymax - Ymin;

            // update the live trend with the new value
            int index = ((n % 200) + 1) % 200;

            UpdateStaticElements(Value, H, rng);
            UpdateAlarmStatus(Value, DriftingSensorValue, H, rng);
            UpdateNoiseModel(Value, DriftingSensorValue, H, rng, index);
            UpdateTrend();

            if (NumSensors > 1)
            {
                UpdateCIFill();
            }
            else
            {
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
//            double Lmargin = 65.0;
//            double Rmargin = 15.0;
            double Tmargin = 0.0;
            double Bmargin = 0.0;
            double TextBlockHeight = 32.0;
            double HighPx = H * ((Ymax - HighAlarmSetpoint) / rng);
            double LowPx = H * (1 - (LowAlarmSetpoint - Ymin) / rng);

            if (LowAlarmSetpoint > Ymin)
            {
                LowAlarmStr = String.Format("{0:0}", LowAlarmSetpoint);
                Tmargin = LowPx - TextBlockHeight / 2.0;
                Tmargin -= LowAlarmLabelVerticalOffset;
                Bmargin = (H - LowPx) - TextBlockHeight / 2.0;
                Bmargin += LowAlarmLabelVerticalOffset;
//                LowAlarmLabel.Margin =
//                    new Thickness(Lmargin, Tmargin, Rmargin, Bmargin);
            }
            if (HighAlarmSetpoint < Ymax)
            {
                Tmargin = HighPx - TextBlockHeight / 2.0;
                Tmargin -= HighAlarmLabelVerticalOffset;
                Bmargin = (H - HighPx) - TextBlockHeight / 2.0;
                Bmargin += HighAlarmLabelVerticalOffset;
                HighAlarmStr = String.Format("{0:0}", HighAlarmSetpoint);
//                HighAlarmLabel.Margin =
//                    new Thickness(Lmargin, Tmargin, Rmargin, Bmargin);
            }

            String PathStr = "";
            if ((LowAlarmSetpoint > Ymin) && (HighAlarmSetpoint < Ymax))
            {
                this.AlarmLimPath.Data = Geometry.Parse(PathStr);
                PathStr = String.Format("M 170,{0:0.0} 180,{0:0.0}", HighPx);
                PathStr += String.Format(" 180,{0:0.0} 170,{0:0.0}", LowPx);
            }
            this.AlarmLimPath.Data = Geometry.Parse(PathStr);

            // Deal with warning labels
            double HighWPx = H * ((Ymax - HighWarningSetpoint) / rng);
            double LowWPx = H * (1 - (LowWarningSetpoint - Ymin) / rng);

            if ((LowWarningSetpoint > Ymin) &&
                (LowWarningSetpoint > LowAlarmSetpoint))
            {
                LowWarningStr = String.Format("{0:0}", LowWarningSetpoint);

                Tmargin = LowWPx - TextBlockHeight / 2.0;
                Tmargin -= LowWarningLabelVerticalOffset;
                Bmargin = (H - LowWPx) - TextBlockHeight / 2.0;
                Bmargin += LowWarningLabelVerticalOffset;
//                LowWarningLabel.Margin =
//                    new Thickness(Lmargin, Tmargin, Rmargin, Bmargin);

                PathStr = String.Format("M 180,{0:0.0} 170,{0:0.0}", LowWPx);
                this.LowWarningTickPath.Data = Geometry.Parse(PathStr);
            }
            else
            {
                this.LowWarningTickPath.Data = Geometry.Parse("");
            }

            if ((HighWarningSetpoint < Ymax) &&
                (HighWarningSetpoint < HighAlarmSetpoint))
            {
                HighWarningStr = String.Format("{0:0}", HighWarningSetpoint);
                Tmargin = HighWPx - TextBlockHeight / 2.0;
                Tmargin -= HighWarningLabelVerticalOffset;
                Bmargin = (H - HighWPx) - TextBlockHeight / 2.0;
                Bmargin += HighWarningLabelVerticalOffset;
//                HighWarningLabel.Margin =
//                    new Thickness(Lmargin, Tmargin, Rmargin, Bmargin);

                PathStr = String.Format("M 180,{0:0.0} 170,{0:0.0}", HighWPx);
                this.HighWarningTickPath.Data = Geometry.Parse(PathStr);
            }
            else
            {
                this.HighWarningTickPath.Data = Geometry.Parse("");
            }
        }

        private void UpdateAlarmStatus(double Value, double DriftingSensorValue,
                                      double H, double rng)
        {
            double newValue = Value += DriftingSensorValue;
            newValue += Value * (NumSensors - 1);
            newValue /= NumSensors;

            // Update Alarm Fill
            if (newValue <= LowAlarmSetpoint)
            {
                this.AlarmRect.Background = Brushes.Red;
                this.AlarmRect.Opacity = 0.8;
            }
            else if (LowAlarmSetpoint < newValue && newValue <= LowWarningSetpoint)
            {
                this.AlarmRect.Background = Brushes.Yellow;
                this.AlarmRect.Opacity = 0.8;
            }
            else if (HighWarningSetpoint < newValue && newValue <= HighAlarmSetpoint)
            {
                this.AlarmRect.Background = Brushes.Yellow;
                this.AlarmRect.Opacity = 0.8;
            }
            else if (newValue > HighAlarmSetpoint)
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

        private void UpdateNoiseModel(double Value, double DriftingSensorValue,
                                      double H, double rng, int index)
        {
            // Model Sensor Noise

            // Draw sample noise from normal distribution using
            // User specified standard deviation
            double[] SampleNoise = new double[(int)NumSensors];
            for (int i = 0; i < NumSensors; i++)
            {
                SampleNoise[i] = Normal(NoiseStd);

                if (i == 0)
                    SampleNoise[i] += DriftingSensorValue;
            }

            // Average the sensor noises together
            SensorNoise[n % noiseN] = SampleNoise.Average();

            // Add to actual value to create noisy value
            NoisyValue = Value + SensorNoise.Average();

            // Scale to pixel units
            double NewValue = (H - H * ((NoisyValue - Ymin) / rng));
            if (NewValue > H) NewValue = H;
            if (NewValue < 0) NewValue = 0;

            // Store NewValue to List
            Data[index] = NewValue;

            // Calculate 90% CI Bound
            double ci90 = SampleStdDev(SampleNoise);
            ci90 /= Math.Sqrt(NumSensors);
            ci90 *= 1.645;

            // Store in pixel units relative to container
            CIData[index] = H * (ci90 / rng);

        }

        private void UpdateTrend()
        {

            // Rebuild the Data path
            // Might be a better way to do this without having to 
            // build the path string first
            var PathStr = new StringBuilder();
            PathStr.Append("M");
            for (int i = 1; i < 200; i++)
            {
                var index = ((n % 200) + 1 + i) % 200;
                PathStr.Append(String.Format(" {0:0},{1:0}", i, Data[index]));
            }

            this.Trend.Data = Geometry.Parse(PathStr.ToString());
            ValueStr = String.Format("{0:0} ", NoisyValue);
        }

        private void UpdateCIFill()
        {
            // Rebuild the Data path
            // Might be a better way to do this without having to 
            // build the path string first
            var PathStr = new StringBuilder();
            PathStr.Append("M");
            for (int i = 1; i < 200; i++)
            {
                var index = ((n % 200) + 1 + i) % 200;
                PathStr.Append(String.Format(" {0:0},{1:0}", i,
                                             Data[index] + CIData[index]));
            }

            for (int i = 199; i > 0; i--)
            {
                var index = ((n % 200) + 1 + i) % 200;
                PathStr.Append(String.Format(" {0:0},{1:0}", i,
                                             Data[index] - CIData[index]));
            }
            this.CI2.Data = Geometry.Parse(PathStr.ToString());
            ValueStr = String.Format("{0:0} ", NoisyValue);
        }
    }
}
