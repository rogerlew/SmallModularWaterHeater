using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterHeater
{
    class Model
    {
        public const double Mout = 600.0 / 16.0;  // mass out in lb/hour
        public const double SHw = 0.999;          // specific heat of water (BTU/lb/F)
        public const double UA = 4.0;            // standby heat loss coefficient * 
                                                  // area of storage tank (BTU/F/hr)
                                                  // 4 = 2.5" insulation; 2.5 = 3" insulation

        public const double Tin = 55.0;           // input water temp (F)
        public const double Tamb = 62.0;          // ambient temperature (F)
        public const double Qon = 1500 * 3.41214; // heat input rate of heater (BTU/hr)
        public const double Min = 1800.0 / 16.0;  // rate of water recover (lb/hr);

        public const double MCapacity = 50 * 128 / 16; // Tank Capacity in lb
        public const double MHigh = MCapacity * 0.74;  // Valve close setpoint (lb)
        public const double MLow = MCapacity * 0.26;    // Valve open setpoint (lb)

        public const double ThermostatHigh = 129.5;  // heater turnoff setpoint (F)
        public const double ThermostatLow = 121.5;   // heater turnon setpoint (F)

        public const int sr = 240;          // samples per hour
        public const double dt = 1.0 / sr;  // time between steps (hours)

        Random random;

        public double TempOld;
        public double TempCur;

        public double MinCur;
        public double MoutCur;
        
        public double TankLevel;
        public double MassOld;
        public double MassCur;
        public double Q;

        public String InValveMode = "Auto";
        public String HeaterMode = "Auto";

        public bool ValveState = false;
        public bool HeaterState = false;
        public bool SCRAMed = false;

        public int n;
        public double t;

        public Model()
        {
            random = new Random();

            // initial temp of tank in F
            double T0 = RandBetween(ThermostatLow, ThermostatHigh);
            TempOld = T0; // initial temperature (F)
            TempCur = T0; // controller needs two samples
            
            // initial level of tank in Percentage
            
            double M0 = RandBetween(MLow, MCapacity);

            TankLevel = 100 * M0 / MCapacity; 
            MassOld = M0; // initial mass (lb)
            MassCur = M0; // controller needs two samples
            
            n = 0;
            t = (double)n * dt;
        }

        private double RandBetween(double low, double high)
        {
            Debug.Assert(high > low);
            return random.NextDouble() * 0.5 * (high - low) + low;
        }

        private double Normal(double mean, double stdDev)
        {
            // http://stackoverflow.com/questions/218060/random-gaussian-variables

            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1))
                                   * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        private double InValveController()
        {
            if (SCRAMed)
                return 0.0;

            switch (InValveMode)
            {
                case "Auto":
                    return controller(MassCur, MassOld, MLow, MHigh) * Min;
                case "Open":
                    return Min;
                default:
                    return 0.0;
            }
        }

        private double TempController()
        {
            if (SCRAMed)
                return 0.0;
            
            switch (HeaterMode)
            {
                case "Auto":
                    return controller(TempCur, TempOld, 
                                      ThermostatLow, ThermostatHigh) * Qon;
                case "On":
                    return Qon;
                default:
                    return 0;
            }
        }

        public void Update()
        {
            // Apply Inlet Flow
            MinCur = InValveController();

            double MassDiff = MinCur / (double)sr;
            double MassNew = MassCur + MassDiff;

            // If mass exceeds capacity of tank truncate mass difference
            if (MassNew > MCapacity)
            {
                MassDiff = MassNew - MCapacity;
                MassNew = MCapacity;
                MinCur = MoutCur;
            }

            // Update tank temperature
            if (MassDiff > 0.0)
            {
                TempCur = (TempCur * MassCur + Tin * MassDiff) / MassNew;
            }

            // Apply outlet flow
            MoutCur = Mout + 40.0 * Math.Sin(6.283185 * 0.001 * t)
                           + 20.0 * Math.Sin(0.9313 * 6.283185 * 0.007 * t)
                           + 10.0 * Math.Sin(0.423 * 6.283185 * 0.011 * t);

            MassNew -= MoutCur / (double)sr;

            // if tank is empty, update state variables and return
            if (MassNew <= 0.0)
            {
                // Update Counter and state variables
                n += 1;
                t = (double)n * dt;

                TankLevel = 0.0;
                MassOld = 0.0;
                MassCur = 0.0;
                MoutCur = 0.0;
//                TempOld = 0.0;
//                TempCur = 0.0;

                ValveState = MassDiff > 0;
                HeaterState = Q > 0;

                return;
            }

            // Find heater input
            Q = TempController();

            // Apply difference equation to compensate
            // for heat loss and heater input
            double TempNew = (-UA * TempCur + UA * Tamb + Q) *
                                dt / (MassNew * SHw) + TempCur;

            // Update Counter and state variables
            n += 1;
            t = (double)n * dt;

            TankLevel = MassNew / MCapacity * 100.0;
            MassOld = MassCur;
            MassCur = MassNew;

            TempOld = TempCur;
            TempCur = TempNew;

            ValveState = MassDiff > 0;
            HeaterState = Q > 0;
        }

        private double controller(double Cur, double Prev, 
                                  double Low, double High)
        {
            // Simple setpoint controller
            if ((Cur - Prev > 0 && Cur < High) || (Cur < Low))
            {
                return 1.0;
            }

            return 0.0;

        }

    }
}
