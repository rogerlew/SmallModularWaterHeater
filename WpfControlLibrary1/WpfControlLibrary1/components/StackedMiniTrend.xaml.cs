﻿using System;
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
    /// Interaction logic for StackedMiniTrend.xaml
    /// </summary>
    public partial class StackedMiniTrend : UserControl
    {

        // n is used to keep track of the last updated index of Data
        // gets reset at 199 so it doesn't need to be a double
        public int n;

        public double Additive;


        // Data stores the trend values is list of ints that get mapped 
        // directly to the Grid Container Coordinates
        public double[] Data1;
        public double[] Data2;
        public double[] Data3;
        public double[] Data4;
        Random random;


        public StackedMiniTrend()
        {
            InitializeComponent();

            
            random = new Random();

            // Initialize Data array to container's height
            double H = this.Container.ActualHeight;
            n = 0;
            Additive = 0;
            Data1 = new double[200];
            Data2 = new double[200];
            Data3 = new double[200];
            Data4 = new double[200];
            for (int i = 0; i < 200; i++)
            {
                Data1[i] = H;
                Data2[i] = H;
                Data3[i] = H;
                Data4[i] = H;
            }

        }

        #region DependencyProperty Declarations
        // Title <string>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
                                        typeof(StackedMiniTrend));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Units <string>
        public static DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(string),
                                        typeof(StackedMiniTrend));

        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        // ValueStr <string>
        public static DependencyProperty ValueStrProperty =
            DependencyProperty.Register("ValueStr", typeof(string),
                                        typeof(StackedMiniTrend));

        public string ValueStr
        {
            get { return (string)GetValue(ValueStrProperty); }
            set { SetValue(ValueStrProperty, value); }
        }

        // YminStr <string>
        public static DependencyProperty YminStrProperty =
            DependencyProperty.Register("YminStr", typeof(string),
                                        typeof(StackedMiniTrend));

        public string YminStr
        {
            get { return (string)GetValue(YminStrProperty); }
            set { SetValue(YminStrProperty, value); }
        }

        // YmaxStr <string>
        public static DependencyProperty YmaxStrProperty =
            DependencyProperty.Register("YmaxStr", typeof(string),
                                        typeof(StackedMiniTrend));

        public string YmaxStr
        {
            get { return (string)GetValue(YmaxStrProperty); }
            set { SetValue(YmaxStrProperty, value); }
        }

        // Ymin <double>
        public static DependencyProperty YminProperty =
            DependencyProperty.Register("Ymin", typeof(double),
                                        typeof(StackedMiniTrend),
                                        new PropertyMetadata((object)0.0));

        public double Ymin
        {
            get { return (double)GetValue(YminProperty); }
            set { SetValue(YminProperty, (double)value); }
        }

        // Ymax <double>
        public static DependencyProperty YmaxProperty =
            DependencyProperty.Register("Ymax", typeof(double),
                                        typeof(StackedMiniTrend),
                                        new PropertyMetadata((object)100.0));

        public double Ymax
        {
            get { return (double)GetValue(YmaxProperty); }
            set { SetValue(YmaxProperty, (double)value); }
        }

        // NoiseStd <double>
        public static DependencyProperty NoiseStdProperty =
            DependencyProperty.Register("NoiseStd", typeof(double),
                                        typeof(StackedMiniTrend),
                                        new PropertyMetadata((object)0.0));

        public double NoiseStd
        {
            get { return (double)GetValue(NoiseStdProperty); }
            set { SetValue(NoiseStdProperty, (double)value); }
        }

        // NumSensors <double>
        public static DependencyProperty NumSensorsProperty =
            DependencyProperty.Register("NumSensors", typeof(double),
                                        typeof(StackedMiniTrend),
                                        new PropertyMetadata((object)1.0));
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

        private double TransformValue(double Value, double H, double rng)
        {
            // Scale to pixel units
            double NewValue = (H - H * ((Value - Ymin) / rng));
            if (NewValue > H) NewValue = H;
            if (NewValue < 0) NewValue = 0;

            return NewValue;
        }

        private double SensorNoise()
        {
            if (random.NextDouble() < 0.1)
                return Normal(NoiseStd);

            return 0.0;
        }

        public void Update(double Value1, double Value2, double Value3, double Value4)
        {
            double NoisyValue1 = Value1 + SensorNoise();
            double NoisyValue2 = Value2 + SensorNoise();
            double NoisyValue3 = Value3 + SensorNoise();
            double NoisyValue4 = Value4 + SensorNoise();

            double H = Container.ActualHeight;
            double rng = Ymax - Ymin;

            double TransformedValue1 = TransformValue(NoisyValue1 * 25.0, H, rng);
            double TransformedValue2 = TransformValue(NoisyValue2 * 25.0, H, rng);
            double TransformedValue3 = TransformValue(NoisyValue3 * 25.0, H, rng);
            double TransformedValue4 = TransformValue(NoisyValue4 * 25.0, H, rng);

            // update the live trend with the new value
            int index = ((n % 200) + 1) % 200;

            // Store NewValue to List
            Data1[index] = TransformedValue1;
            Data2[index] = Data1[index] - (H - TransformedValue2);
            Data3[index] = Data2[index] - (H - TransformedValue3);
            Data4[index] = Data3[index] - (H - TransformedValue4);

            UpdateTrendFill(Data1, Fill1);
            UpdateTrendFill(Data2, Fill2);
            UpdateTrendFill(Data3, Fill3);
            UpdateTrendFill(Data4, Fill4);
            UpdateTrendLine(Data4, Trend);

            // Update the live text value
            Additive = 100 * (Value1 + Value2 + Value3 + Value4) / 4;
            ValueStr = String.Format("{0:0} ", Additive);

            YminStr = String.Format("{0}", Ymin);
            YmaxStr = String.Format("{0}", Ymax);

            // update the counter
            n++;
            n %= 200;
        }

        private void UpdateTrendLine(double[] Data, Path Trend)
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
            Trend.Data = Geometry.Parse(PathStr.ToString());
        }

        private void UpdateTrendFill(double[] Data, Path Fill)
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
                                             Data[index]));
            }

            for (int i = 199; i > 0; i--)
            {
                var index = ((n % 200) + 1 + i) % 200;
                PathStr.Append(String.Format(" {0:0},150", i));
            }
            Fill.Data = Geometry.Parse(PathStr.ToString());
        }
    }
}
