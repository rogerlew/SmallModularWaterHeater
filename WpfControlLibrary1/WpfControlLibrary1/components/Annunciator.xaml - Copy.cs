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


        public Annunciator()
        {
            InitializeComponent();
        }

        public void Update(double Value)
        {
            // If alarm is already triggered we should return,
            // Otherwise the Blinking just gets retriggered
            if (AlarmState)
                return;

            if ((Value < LowAlarmSetpoint) || (Value > HighAlarmSetpoint))
            {
                AlarmState = true;
                _Update();
            }
        }

        private void _Update()
        {
            this.AlarmTextBlock.Text = AlarmLabel;

            if (AlarmState && !AlarmAcknowledged)
            {
                Storyboard sbdAlarmBlinking = (Storyboard)FindResource("sbdAlarmBlinking");
                sbdAlarmBlinking.Begin();

            }
            else if (AlarmState && AlarmAcknowledged)
            {
                Storyboard sbdAlarmBlinking = (Storyboard)FindResource("sbdAlarmBlinking");
                sbdAlarmBlinking.Stop();
                AlarmRect.Opacity = 1.0;
            }
            else
            {
                Storyboard sbdAlarmBlinking = (Storyboard)FindResource("sbdAlarmBlinking");
                sbdAlarmBlinking.Stop();
                AlarmRect.Opacity = 0.0;
            }

        }

        public void Trigger()
        {
            AlarmState = true;
            _Update();
        }

        public void Silence()
        {
        }
        
        public void Acknowledge()
        {
            AlarmAcknowledged = true;
            _Update();
        }

        public void Reset()
        {
            AlarmState = false;
            AlarmAcknowledged = false;
            _Update();
        }
    }
}
