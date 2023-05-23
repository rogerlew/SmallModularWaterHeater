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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfControlLibrary1
{
    /// <summary>
    /// Interaction logic for Annunciator.xaml
    /// </summary>
    public partial class Annunciator : UserControl
    {
        public const String DefaultLabel = "THE VALVE IS STUCK OPEN BROSEF";

        // AlarmState <bool>
        public static DependencyProperty AlarmStateProperty =
            DependencyProperty.Register("AlarmState", typeof(bool), 
                                        typeof(Annunciator),
                                        new PropertyMetadata(false));

        public bool AlarmState
        {
            get { return (bool)GetValue(AlarmStateProperty); }
            set { SetValue(AlarmStateProperty, value); }
        }

        // AlarmAcknowledged <bool>
        public static DependencyProperty AlarmAcknowledgedProperty =
            DependencyProperty.Register("AlarmAcknowledged", typeof(bool), 
                                        typeof(Annunciator),
                                        new PropertyMetadata(false));

        public bool AlarmAcknowledged
        {
            get { return (bool)GetValue(AlarmAcknowledgedProperty); }
            set { SetValue(AlarmAcknowledgedProperty, value); }
        }

        // Silenced <bool>
        public static DependencyProperty SilencedProperty =
            DependencyProperty.Register("Silenced", typeof(bool),
                                        typeof(Annunciator),
                                        new PropertyMetadata(false));

        public bool Silenced
        {
            get { return (bool)GetValue(SilencedProperty); }
            set { SetValue(SilencedProperty, value); }
        }

        // AlarmColor <Brush>
        public static DependencyProperty AlarmColorProperty =
            DependencyProperty.Register("AlarmColor", typeof(Brush), 
                                        typeof(Annunciator),
                                        new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush AlarmColor
        {
            get { return (Brush)GetValue(AlarmColorProperty); }
            set { SetValue(AlarmColorProperty, value); }
        }

        // AlarmLabel <string>
        public static DependencyProperty AlarmLabelProperty =
            DependencyProperty.Register("AlarmLabel", typeof(string), 
                                        typeof(Annunciator),
                                        new PropertyMetadata(DefaultLabel));

        public string AlarmLabel
        {
            get { return (string)GetValue(AlarmLabelProperty); }
            set { SetValue(AlarmLabelProperty, value); }
        }

        // OscillatorValue <double>
        public static DependencyProperty OscillatorValueProperty =
            DependencyProperty.Register("OscillatorValue", typeof(double),
                                        typeof(Annunciator),
                                        new PropertyMetadata((object)-1e38));

        public double OscillatorValue
        {
            get { return (double)GetValue(OscillatorValueProperty); }
            set { SetValue(OscillatorValueProperty, (double)value); }
        }

        // LowAlarmSetpoint <double>
        public static DependencyProperty LowAlarmSetpointProperty =
            DependencyProperty.Register("LowAlarmSetpoint", typeof(double),
                                        typeof(Annunciator),
                                        new PropertyMetadata((object)-1e38));

        public double LowAlarmSetpoint
        {
            get { return (double)GetValue(LowAlarmSetpointProperty); }
            set { SetValue(LowAlarmSetpointProperty, (double)value); }
        }

        // HighAlarmSetpoint <double>
        public static DependencyProperty HighAlarmSetpointProperty =
            DependencyProperty.Register("HighAlarmSetpoint", typeof(double),
                                        typeof(Annunciator),
                                        new PropertyMetadata((object)1e38));

        public double HighAlarmSetpoint
        {
            get { return (double)GetValue(HighAlarmSetpointProperty); }
            set { SetValue(HighAlarmSetpointProperty, (double)value); }
        }

        // BlinkRate <double>
        // Default is 0.5 Hz
        // 10 % (1 / BlinkRate) should be an Integer
        public static DependencyProperty BlinkRateProperty =
            DependencyProperty.Register("BlinkRate", typeof(double),
                                        typeof(Annunciator),
                                        new PropertyMetadata((object)0.5));

        public double BlinkRate
        {
            get { return (double)GetValue(BlinkRateProperty); }
            set { SetValue(BlinkRateProperty, (double)value); }
        }
        
        public DispatcherTimer dispatcherTimer;

        public Annunciator()
        {

            // Setup Timer for Event Loop
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Oscillator);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1.0/30.0);
            dispatcherTimer.Start();


            InitializeComponent();
        }

        private double ClippedSineWave()
        {
            // Returns the value of a clipped sine wave based on the
            // system clock with frequency set by the BlinkRate 
            // DependencyProperty

            String tstr = DateTime.Now.ToString("s.fff");
            double t = Convert.ToDouble(tstr) % 10.0;
            double ret = Math.Sin(BlinkRate * 6.28318531 * t) + 0.5;

            if (ret > 1.0)
            {
                return 1.0;
            }
            else if (ret < 0.0)
            {
                return 0.0;
            }
            return ret;
        }

        private void Oscillator(object sender, EventArgs e)
        {
            // AlarmRect.Opacity is bound to OscillatorValue

            if (AlarmState)
            {
                if (AlarmAcknowledged)
                {
                    OscillatorValue = 1.0;
                }
                else
                {
                    OscillatorValue = ClippedSineWave();
                }
            }
            else
            {
                OscillatorValue = 0.0;
            }

        }

        public void Update(double Value)
        {
            // If alarm is already triggered we should return,
            // Otherwise the Blinking just gets retriggered
            if (AlarmState)
                return;

            if ((Value < LowAlarmSetpoint) || (Value > HighAlarmSetpoint))
                AlarmState = true;
            
        }

//        private void _Update()
//        {
//            this.AlarmTextBlock.Text = AlarmLabel;
//        }
          
        public void Trigger()
        {
            AlarmState = true;
        }

        public void Silence()
        {
            if (AlarmState) 
                Silenced = true;
        }
        
        public void Acknowledge()
        {
            if (AlarmState)
            {
                Silenced = true;
                AlarmAcknowledged = true;
            }
        }

        public void Reset()
        {
            AlarmState = false;
            Silenced = false;
            AlarmAcknowledged = false;
        }
    }

}
