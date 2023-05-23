using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using IniParser;

namespace WaterHeater
{
    class Model
    {
        public double SHw;
        public double UA;
        public double Tamb;
        public double Qon;

        public double MCapacity;
        public double MHigh;
        public double MLow;

        public double AccumMCapacity;
        public double AccumMHigh;
        public double AccumMLow;

        public double ThermostatHigh;
        public double ThermostatLow;

        public double DiverterDebounce;
        public double DiverterLow;
        public double DiverterLastChanged;

        public double sr;
        public double dt;
        public double tMultiplier;
        public double PumpLeakRate;

        Random random;

        public double TempOld;
        public double TempCur;

        public double MinCur;
        public double MoutCur;
        public double AccumAoutCur;
        public double AccumBoutCur;

        public double TankLevel;
        public double MassOld;
        public double MassCur;
        public double Q;

        public double AccumALevel;
        public double AccumBLevel;
        public double AccumAMassOld;
        public double AccumBMassOld;
        public double AccumAMassCur;
        public double AccumBMassCur;

        public double dTankMass;
        public double dTankTemp;
        public double dAccumAMass;
        public double dAccumBMass;

        public double ttTankMassHigh = 0.0;
        public double ttTankMassLow = 0.0;
        public double ttTankTempHigh = 0.0;
        public double ttTankTempLow = 0.0;
        public double ttAccumAMassHigh = 0.0;
        public double ttAccumAMassLow = 0.0;
        public double ttAccumBMassHigh = 0.0;
        public double ttAccumBMassLow = 0.0;

        public String PumpMode;
        public String InValveMode;
        public String HeaterMode;
        public String DiverterValveMode;

        public bool PumpState;
        public bool InValveState;
        public bool HeaterState;
        public bool DiverterValveState;

        public bool SCRAMed;

        public int n;
        public double t;

        public Model(String ConfigFileName)
        {
            // Read Configuration file
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.LoadFile(ConfigFileName);
            
            // GeneralConfiguration
            tMultiplier = Convert.ToDouble(data["GeneralConfiguration"]["tMultiplier"]);
            SHw = Convert.ToDouble(data["GeneralConfiguration"]["SHw"]);
            Tamb = Convert.ToDouble(data["GeneralConfiguration"]["Tamb"]);
            SCRAMed = Convert.ToBoolean(data["GeneralConfiguration"]["SCRAMed"]);

            // WaterHeater
            UA = Convert.ToDouble(data["WaterHeater"]["UA"]);
            MCapacity = Convert.ToDouble(data["WaterHeater"]["MCapacity"]);
            MHigh = Convert.ToDouble(data["WaterHeater"]["MHigh"]);
            MLow = Convert.ToDouble(data["WaterHeater"]["MLow"]);

            // Accumulators
            AccumMCapacity = Convert.ToDouble((String)(data["Accumulators"]["AccumMCapacity"]));
            AccumMHigh = Convert.ToDouble(data["Accumulators"]["AccumMHigh"]);
            AccumMLow = Convert.ToDouble(data["Accumulators"]["AccumMLow"]);

            // InValveController
            InValveMode = data["InValveController"]["InValveMode"];
            InValveMode = InValveMode.Substring(1, InValveMode.Length - 2);
            Console.WriteLine("'" + InValveMode + "'");
            if (InValveMode == "Auto" || InValveMode == "On")
                InValveState = true;
            else
                InValveState = false;

            // PumpController
            PumpMode = data["PumpController"]["PumpMode"];
            PumpMode = PumpMode.Substring(1, PumpMode.Length - 2);
            if (PumpMode == "Auto" || PumpMode == "On")
                PumpState = true;
            else
                PumpState = false;
            PumpLeakRate = Convert.ToDouble(data["PumpController"]["PumpLeakRate"]);

            // HeaterController
            if (HeaterMode == "Auto" || HeaterMode == "Off")
                HeaterState = false;
            else
                HeaterState = true;
            HeaterMode = data["HeaterController"]["HeaterMode"];
            HeaterMode = HeaterMode.Substring(1, HeaterMode.Length - 2);
            Qon = Convert.ToDouble(data["HeaterController"]["Qon"]);
            ThermostatHigh = Convert.ToDouble(data["HeaterController"]["ThermostatHigh"]);
            ThermostatLow = Convert.ToDouble(data["HeaterController"]["ThermostatLow"]);

            // DiverterValveController 
            DiverterValveMode = data["DiverterValveController"]["DiverterValveMode"];
            DiverterValveMode = DiverterValveMode.Substring(1, DiverterValveMode.Length - 2);
            if (DiverterValveMode == "Auto" || DiverterValveMode == "Accumulator A")
                DiverterValveState = false;
            else
                DiverterValveState = true;
            DiverterDebounce = Convert.ToDouble(data["DiverterValveController"]["DiverterDebounce"]);
            DiverterLow = Convert.ToDouble(data["DiverterValveController"]["DiverterLow"]);

            _Model();
        }
        private void _Model()
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

            M0 = 80;
            AccumALevel = 100 * M0 / AccumMCapacity;
            AccumAMassOld = M0;
            AccumAMassCur = M0;

            AccumBLevel = 100 * M0 / AccumMCapacity;
            AccumBMassOld = M0;
            AccumBMassCur = M0;

            n = 0;
            t = 0.0;
            DiverterLastChanged = t;
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

        private double InValveController(double Mavailable)
        {
            if (SCRAMed)
                return 0.0;

            switch (InValveMode)
            {
                case "Auto":
                    return controller(MassCur, MassOld, MLow, MHigh)
                           * Mavailable;
                case "Open":
                    return Mavailable;
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

        private double PumpController(double TankOutFlow)
        {
            if (SCRAMed)
                return 0;

            switch (PumpMode)
            {
                case "Auto":
                    if (!DiverterValveState)
                        return controller(AccumAMassCur, AccumAMassOld, 
                                          AccumMLow, AccumMHigh)
                               * TankOutFlow;
                    else
                        return controller(AccumBMassCur, AccumBMassOld, 
                                          AccumMLow, AccumMHigh)
                               * TankOutFlow;

                case "On":
                    return TankOutFlow;
                default:
                    return 0;
            }
        }

        private bool DiverterController()
        {
            if ((t - DiverterLastChanged) < DiverterDebounce)
                return DiverterValveState;

            bool newDiverterState;

            switch (DiverterValveMode)
            {
                case "Auto":
                    if ((AccumALevel < DiverterLow) && (AccumBLevel < DiverterLow))
                        newDiverterState = (AccumALevel > AccumBLevel);
                    else if (AccumALevel < DiverterLow)
                        newDiverterState = false;
                    else if (AccumBLevel < DiverterLow)
                        newDiverterState = true;
                    else
                        newDiverterState = DiverterValveState;
                    break;

                case "Accumulator A":
                    newDiverterState = false;
                    break;

                default:
                    newDiverterState = true;
                    break;
            }
            
            if (DiverterValveState != newDiverterState)
                DiverterLastChanged = t;

            return newDiverterState;
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

        public void Update(double tnew,
                           double Mavailable, // rate of available input water
                           double Tavailable, // temp of available input water
                           double TankOutFlow,
                           double AccumARequestedOut,  // requested output rate from Accumulator A
                           double AccumBRequestedOut)  // requested output rate from Accumulator B
        {
            // Admittedly, not the most elegant code that I have written,
            // but it gets the job done and seems to work reliably.
            tnew *= tMultiplier;
            dt = tnew - t;
            sr = 1.0 / dt;
            t = tnew;

            // Apply Inlet Flow
            MinCur = InValveController(Mavailable);

            double MassDiff = MinCur / sr;
            double MassNew = MassCur + MassDiff;

            DiverterValveState = DiverterController();
            MoutCur = PumpController(TankOutFlow);

            // If mass exceeds capacity of tank truncate mass difference
            if (MassNew > MCapacity)
            {
                MassDiff = MassNew - MCapacity;
                MassNew = MCapacity;
                MinCur = MoutCur;
            }

            MassNew -= MoutCur / sr;

            // Update tank temperature
            if (MassDiff > 0.0)
            {
                TempCur = (TempCur * (MassCur - MoutCur / sr) + Tavailable * MassDiff) / MassNew;
            }

            double TempNew = TempOld;

            // if tank is empty, update state variables and return
            if (MassNew <= 0.0)
            {
                
                MassNew = 0.0;
                MassOld = 0.0;
                MassCur = 0.0;

                InValveState = MassDiff > 0;
                HeaterState = Q > 0;

            }
            else
            {
                // Find heater input
                Q = TempController();

                // Apply difference equation to compensate
                // for heat loss and heater input
                TempNew = (-UA * TempCur + UA * Tamb + Q) *
                          dt / (MassNew * SHw) + TempCur;
            }

            double AccumAMinCur = 0;
            double AccumBMinCur = 0;
            AccumAoutCur = AccumARequestedOut;
            AccumBoutCur = AccumBRequestedOut;

            // Find accumulator inflow

            if (!DiverterValveState)
            {
                if (!PumpState)
                    AccumAMinCur = Normal(PumpLeakRate, 0.3);
                else
                    AccumAMinCur = MoutCur;

                AccumBMinCur = 0;
            }
            else
            {
                if (!PumpState)
                    AccumBMinCur = Normal(PumpLeakRate, 0.3);
                else
                    AccumBMinCur = MoutCur;

                AccumAMinCur = 0;
            }

            double AccumAMassDiff = (AccumAMinCur-AccumAoutCur) / sr;
            double AccumAMassNew = AccumAMassCur + AccumAMassDiff;

            // If mass exceeds capacity of tank truncate mass difference
            if (AccumAMassNew >= AccumMCapacity)
            {
                AccumAMassDiff = AccumAMassNew - AccumMCapacity;
                AccumAMassNew = AccumMCapacity;
                AccumAMinCur = AccumAoutCur;
                MoutCur = AccumAoutCur;

                if (MassNew >= MCapacity * 0.99)
                    MinCur = MoutCur;
            }


            if (AccumAMassNew <= 0.0)
            {
                AccumAMassNew = 0.0;
                AccumAMassCur = 0.0;
                AccumAoutCur = 0.0;
                if (!DiverterValveState)
                {
                    if (MoutCur > AccumARequestedOut)
                    {
                        AccumAoutCur = AccumARequestedOut;
                        AccumAMassNew = (MoutCur - AccumARequestedOut) / sr;
                    }
                    else
                    {
                        AccumAoutCur = MoutCur;
                    }
                }
            }

            double AccumBMassDiff = (AccumBMinCur - AccumBoutCur) / sr;
            double AccumBMassNew = AccumBMassCur + AccumBMassDiff;

            // If mass exceeds capacity of tank truncate mass difference
            if (AccumBMassNew >= AccumMCapacity)
            {
                AccumBMassDiff = AccumBMassNew - AccumMCapacity;
                AccumBMassNew = AccumMCapacity;
                AccumBMinCur = AccumBoutCur;
                MoutCur = AccumBoutCur;

                if (MassNew >= MCapacity * 0.99)
                    MinCur = MoutCur;

            }

            if (AccumBMassNew <= 0.0)
            {
                AccumBMassNew = 0.0;
                AccumBMassCur = 0.0;
                AccumBoutCur = 0.0;
                if (DiverterValveState)
                {
                    if (MoutCur > AccumBRequestedOut)
                    {
                        AccumBoutCur = AccumBRequestedOut;
                        AccumBMassNew = (MoutCur - AccumBRequestedOut) / sr;
                    }
                    else
                    {
                        AccumBoutCur = MoutCur;
                    }
                }
            }


            // Update Counter and state variables
            n += 1;

            TankLevel = MassNew / MCapacity * 100.0;
            MassOld = MassCur;
            MassCur = MassNew;

            TempOld = TempCur;
            TempCur = TempNew;

            AccumALevel = AccumAMassNew / AccumMCapacity * 100.0;
            AccumBLevel = AccumBMassNew / AccumMCapacity * 100.0;

            AccumAMassOld = AccumAMassCur;
            AccumBMassOld = AccumBMassCur;

            AccumAMassCur = AccumAMassNew;
            AccumBMassCur = AccumBMassNew;

            InValveState = MassDiff > 0;
            HeaterState = Q > 0;

            dTankMass = ((MassCur - MassOld) / dt);
            ttTankMassHigh = (MHigh - MassCur) / dTankMass;
            ttTankMassLow = (MLow - MassCur) / dTankMass;

            dTankTemp = ((TempCur - TempOld) / dt);
            ttTankTempHigh = (ThermostatHigh - TempCur) / dTankTemp;
            ttTankTempLow = (ThermostatLow - TempCur) / dTankTemp;

            dAccumAMass = ((AccumAMassCur - AccumAMassOld) / dt);
            ttAccumAMassHigh = (AccumMHigh - AccumAMassCur) / dAccumAMass;
            ttAccumAMassLow = (AccumMLow - AccumAMassCur) / dAccumAMass;

            dAccumBMass = ((AccumBMassCur - AccumBMassOld) / dt);
            ttAccumBMassHigh = (AccumMHigh - AccumBMassCur) / dAccumBMass;
            ttAccumBMassLow = (AccumMLow - AccumBMassCur) / dAccumBMass;
        }


    }
}



//            // Apply outlet flow
//            MoutCur = Mout + 40.0 * Math.Sin(6.283185 * 0.001 * t)
//                           + 20.0 * Math.Sin(0.9313 * 6.283185 * 0.007 * t)
//                           + 10.0 * Math.Sin(0.423 * 6.283185 * 0.011 * t);
