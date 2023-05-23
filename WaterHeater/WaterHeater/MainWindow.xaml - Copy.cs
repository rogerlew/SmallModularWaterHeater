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
using System.Windows.Threading;

namespace WaterHeater
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static double Mout = 600.0 / 16.0;          // mass out in lb/hour
        public static double SHw = 0.999;       // specific heat of water (BTU/lb/F)
        public static double UA = 4;            // standby heat loss coefficient * area of storage tank (BTU/F/hr)
                                                // 4 = 2.5" insulation; 2.5 = 3" insulation
        public static double Tamb = 30;         // ambient temperature (F)
        public static double Tin  = 60;         // input water temp
        public static double Qon = 400 * 3.41214; // rate of heat input to tank from the heater (BTU/hr);
        public static double Mon = 1800.0 / 16.0;  // rate of water recover (lb/hr);

        public static double MHigh = 80 * 128 / 16.0;
        public static double MLow = MHigh * 0.05;

        public static double ThermostatHigh = 140.0;
        public static double ThermostatLow = 135.0;

        DispatcherTimer timerdigital;

        public MainWindow()
        {
            InitializeComponent();

            Random random = new Random();

            int duration = 100; // hours
            int sr = 60;      // samples per hour
            int N = sr*duration;
            double dt = (double)duration / (double)N; // time between steps in hours

            double TempOld = ThermostatHigh; // initial temperature (F)
            double TempCur = ThermostatHigh; // controller needs two samples

            double MassOld = MHigh; // initial mass (lb)
            double MassCur = MHigh; // controller needs two samples

            double Q = 0.0;

            int n = 0;
            double t = (double)n * dt;

            timerdigital = new DispatcherTimer();
            timerdigital.Interval = TimeSpan.FromSeconds(1.0/600.0);
            timerdigital.Start();
            timerdigital.Tick += new EventHandler(delegate(object s, EventArgs a)
            {
                double Mdiff = controller(MassCur, MassOld, MLow, MHigh) * 
                               Mon / (double)sr - Mout / (double)sr;

                double MassNew = MassCur + Mdiff;
                if (MassNew > MHigh)
                {
                    Mdiff = MassNew - MHigh;
                    MassNew = MHigh;
                }
                
                Q = controller(TempCur, TempOld, ThermostatLow, ThermostatHigh)*Qon;

                if (Mdiff > 0.0) {
                    TempCur = (TempCur * MassCur + Tin * Mdiff) / MassNew;
                }
                double TempNew = (-UA * TempCur + UA * Tamb + Q) *
                                 dt / (MassNew * SHw) + TempCur;

                this.TimeLabel.Content = String.Format("{0:0.0}", t);
                this.TempLabel.Content = String.Format("{0:0.0}", TempNew);
                this.MassLabel.Content = String.Format("{0:0}", MassNew);
                this.QLabel.Content = String.Format("{0:0}", Q);

                n += 1;
                t = (double)n * dt;

                MassOld = MassCur;
                MassCur = MassNew;

                TempOld = TempCur;
                TempCur = TempNew;
            });
        }

        private double controller(double Cur, double Prev, double Low, double High)
        {
            var dT = Cur - Prev;

            if (dT > 0 && Cur < High)
            {
                return 1.0;
            }
            else
            {
                if (Cur < Low)
                {
                    return 1.0;
                }
                else
                {
                    return 0.0;
                }
            }
        }
    }
}
