using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Media.Animation;
using CommandLine;
using System.Media;

using IniParser;

using WpfControlLibrary1;
using WaterHeater;

namespace SmallModularWaterHeater
{

    /// <summary>
    /// Handles command line arguments for specifying LOA, the datafile name, and trial duration
    /// </summary>
    class Options
    {
        [Option('l', "loa", Required = true, DefaultValue = "LOA07.ini")]
        public string ConfigFile { get; set; }

        [Option('d', "datafile", Required = true, DefaultValue = "Test-LOA07-SMWH00.csv")]
        public string DataFile { get; set; }

        [Option('t', "time", Required = true, DefaultValue = 600)]
        public int RunTime { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {

        WaterHeater.Model wh01;

        private int nskip;
        private int n = 0;
        private int mskip;
        private int m = 0;
        private DispatcherTimer dispatcherTimer;
        private double t;
        private DateTime t0;
        private List<DSSControl> DSSList;
        private UInt64 DSSHash;
        private const double TempSensorNoiseStd = 2.0;
        
        private Options options;

        private SoundPlayer player;
        private bool isPlaying = false;

        #region Mavailable <double>
        public static DependencyProperty MavailableProperty =
            DependencyProperty.Register("Mavailable", typeof(double),
                                        typeof(Annunciator));

        public double Mavailable
        {
            get { return (double)GetValue(MavailableProperty); }
            set { SetValue(MavailableProperty, (double)value); }
        }
        #endregion

        #region TankOutFlow <double>
        public static DependencyProperty TankOutFlowProperty =
            DependencyProperty.Register("TankOutFlow", typeof(double),
                                        typeof(Annunciator));

        public double TankOutFlow
        {
            get { return (double)GetValue(TankOutFlowProperty); }
            set { SetValue(TankOutFlowProperty, (double)value); }
        }
        #endregion

        #region AccumAout <double>
        public static DependencyProperty AccumAoutProperty =
            DependencyProperty.Register("AccumAout", typeof(double),
                                        typeof(Annunciator));

        public double AccumAout
        {
            get { return (double)GetValue(AccumAoutProperty); }
            set { SetValue(AccumAoutProperty, (double)value); }
        }
        #endregion

        #region AccumBout <double>
        public static DependencyProperty AccumBoutProperty =
            DependencyProperty.Register("AccumBout", typeof(double),
                                        typeof(Annunciator));

        public double AccumBout
        {
            get { return (double)GetValue(AccumBoutProperty); }
            set { SetValue(AccumBoutProperty, (double)value); }
        }
        #endregion

        #region Tin <double>
        public static DependencyProperty TinProperty =
            DependencyProperty.Register("Tin", typeof(double),
                                        typeof(Annunciator));

        public double Tin
        {
            get { return (double)GetValue(TinProperty); }
            set { SetValue(TinProperty, (double)value); }
        }
        #endregion

        #region DSSWarningTime <double>
        public static DependencyProperty DSSWarningTimeProperty =
            DependencyProperty.Register("DSSWarningTime", typeof(double),
                                        typeof(Annunciator));

        public double DSSWarningTime
        {
            get { return (double)GetValue(DSSWarningTimeProperty); }
            set { SetValue(DSSWarningTimeProperty, (double)value); }
        }
        #endregion

        public MainWindow()
        {
            String[] args = Environment.GetCommandLineArgs();

            options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);
            //options.DataFile = System.IO.Path.Combine(@"./Data/", options.DataFile);
            Console.WriteLine(options.DataFile);
                
            // Read Configuration file
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.LoadFile(options.ConfigFile);

            // Setup Timer for Event Loop
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Update);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            dispatcherTimer.Start();

            // Bind Escape key to Close Program
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            // initalize a new water heater model
            wh01 = new WaterHeater.Model(options.ConfigFile);

            // figure out start time
            t0 = DateTime.Now;

            //Initialize DSSList
            DSSList = new List<DSSControl>();
            DSSHash = 1000ul;

            // Initialize soundplayer for DSS

            player = new SoundPlayer(data["DSS"]["AlarmSound"]);

            WriteHeader();

            InitializeComponent();

            SetInValveMode(wh01.InValveMode);
            SetHeaterMode(wh01.HeaterMode);
            SetDiverterValveMode(wh01.DiverterValveMode);
            SetPumpMode(wh01.PumpMode);

            if (data["GeneralConfiguration"]["AnnunciateMinitrends"] != "true")
            {
                LevelIndicator.Annunciation = false;
                TempIndicator.Annunciation = false;
                AccumALevelIndicator.Annunciation = false;
                AccumBLevelIndicator.Annunciation = false;
            }

            nskip = Convert.ToInt32(data["GeneralConfiguration"]["TrendInterval"]);
            mskip = Convert.ToInt32(data["GeneralConfiguration"]["DataCollectionInterval"]);
    
            // SliderControls
            if (data["SliderControls"]["ShowInFlowSldr"] != "true")
                InFlowSldr.Visibility = Visibility.Hidden;

            if (data["SliderControls"]["ShowTinSldr"] != "true")
                TinSldr.Visibility = Visibility.Hidden;

            if (data["SliderControls"]["ShowAccumAOutFlowSldr"] != "true")
                AccumAOutFlowSldr.Visibility = Visibility.Hidden;

            if (data["SliderControls"]["ShowAccumBOutFlowSldr"] != "true")
                AccumBOutFlowSldr.Visibility = Visibility.Hidden;

            // Controls
            if (data["InValveController"]["ShowValveControlPanel"] != "true")
                ValveControlPanel.Visibility = Visibility.Hidden;

            if (data["HeaterController"]["ShowHeaterControlPanel"] != "true")
                HeaterControlPanel.Visibility = Visibility.Hidden;

            if (data["DiverterValveController"]["ShowDiverterValveControlPanel"] != "true")
                DiverterValveControlPanel.Visibility = Visibility.Hidden;

            if (data["PumpController"]["ShowPumpControlPanel"] != "true")
                PumpControlPanel.Visibility = Visibility.Hidden;

            // DSS
            DSSWarningTime = Convert.ToDouble(data["DSS"]["WarningTime"]);

            if (data["DSS"]["ShowDSS"] != "true")
                DSS.Visibility = Visibility.Hidden;
        }

        private void ShowEndMask()
        {
            if (EndMask.Visibility == Visibility.Visible)
                return;

            btnEnd.Content = "Close SMWH01";
            EndMask.Visibility = Visibility.Visible;
            EndDialog.Visibility = Visibility.Visible;
            lblEndDatafileFeedback.Content += options.DataFile;
        }

        private void Update(object sender, EventArgs e)
        {
            // Update time
            var ts = DateTime.Now - t0; 
            t = ts.TotalSeconds;
            this.TimeLabel.Content = ts.ToString();

            // Is the trial over?
            if (t > options.RunTime)
            {
                ShowEndMask();
                return;
            }

            // Update model
            wh01.Update(t, Mavailable, Tin, TankOutFlow, AccumAout, AccumBout);
            
            // Update Annunciators
            UpdateAnnunciators();

            // Update Safety Override System
            UpdateSafetyOverrideInterface();

            // Update DSS Shotclocks
            UpdateShotclocks();

            // Update DSS System
            UpdateDSSOrder();

            // Update the controls
            if (n == 0)
                PeriodicUpdateControls();

            UpdateControls();

            // Write Data
            if (m == 0)
                WriteLine();
            
            // Update Counters
            n++; n %= nskip;
            m++; m %= mskip;
        }

        private void UpdateAnnunciators()
        {
            this.A01.Update(wh01.TankLevel);
            this.A11.Update(wh01.TankLevel);
            this.A21.Update(wh01.TankLevel);
            this.A31.Update(wh01.TankLevel);

            this.A03.Update(wh01.TempCur);
            this.A13.Update(wh01.TempCur);
            this.A23.Update(wh01.TempCur);
            this.A33.Update(wh01.TempCur);

            this.A04.Update(wh01.AccumALevel);
            this.A14.Update(wh01.AccumALevel);
            this.A24.Update(wh01.AccumALevel);
            this.A34.Update(wh01.AccumALevel);

            this.A05.Update(wh01.AccumBLevel);
            this.A15.Update(wh01.AccumBLevel);
            this.A25.Update(wh01.AccumBLevel);
            this.A35.Update(wh01.AccumBLevel);

            if ((A01.AlarmState && !A01.Silenced) ||
                (A11.AlarmState && !A11.Silenced) ||
                (A21.AlarmState && !A21.Silenced) ||
                (A31.AlarmState && !A31.Silenced) ||
                (A03.AlarmState && !A03.Silenced) ||
                (A13.AlarmState && !A13.Silenced) ||
                (A23.AlarmState && !A23.Silenced) ||
                (A33.AlarmState && !A33.Silenced) ||
                (A04.AlarmState && !A04.Silenced) ||
                (A14.AlarmState && !A14.Silenced) ||
                (A24.AlarmState && !A24.Silenced) ||
                (A34.AlarmState && !A34.Silenced) ||
                (A05.AlarmState && !A05.Silenced) ||
                (A15.AlarmState && !A15.Silenced) ||
                (A25.AlarmState && !A25.Silenced) ||
                (A35.AlarmState && !A35.Silenced)
                )
            {
                if (!isPlaying)
                {
                    Console.WriteLine("Starting alarm sound");
                    isPlaying = true;
                    player.PlayLooping();
                }
            }
        }

        private void UpdateSafetyOverrideInterface()
        {
            // Level High Safety Override
            if (wh01.SOLevelHighActuated)
            {
                if (wh01.InValveMode != "Close")
                {
                    InValveCloseBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    PLevelHigh.Update(1);
                }
            }
            else
            {
                if (wh01.InValveMode == "Close" &&
                    !InValveOpenBtn.IsEnabled &&
                    !InValveAutoBtn.IsEnabled &&
                    !InValveCloseBtn.IsEnabled)
                {
                    PLevelHigh.Update(0);
                }

            }

            // Level Low Safety Override
            if (wh01.SOLevelLowActuated)
            {
                if (wh01.InValveMode != "Open")
                {
                    InValveOpenBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    PLevelLow.Update(1);
                }
            }
            else
            {
                if (wh01.InValveMode == "Open" &&
                    !InValveOpenBtn.IsEnabled &&
                    !InValveAutoBtn.IsEnabled &&
                    !InValveCloseBtn.IsEnabled)
                {
                    PLevelLow.Update(0);
                }
            }

            // Temp High Safety Override
            if (wh01.SOTempHighActuated)
            {
                if (wh01.HeaterMode != "Off")
                {
                    HeaterOffBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    PTempHigh.Update(1);
                }
            }
            else
            {
                if (wh01.InValveMode == "Open" &&
                    !HeaterOnBtn.IsEnabled &&
                    !HeaterAutoBtn.IsEnabled &&
                    !HeaterOffBtn.IsEnabled)
                {
                    PTempHigh.Update(0);
                }
            }

            // Accum A Level High Safety Override
            if (wh01.SOAccumALevelHighActuated)
            {
                PAccumALevelHigh.Update(1);
            }
            else
            {
                PAccumALevelHigh.Update(0);
            }

            // Accum A Level Low Safety Override
            if (wh01.SOAccumALevelLowActuated)
            {
                PAccumALevelLow.Update(1);
            }
            else
            {
                PAccumALevelLow.Update(0);
            }

            // Accum B Level High Safety Override
            if (wh01.SOAccumBLevelHighActuated)
            {
                PAccumBLevelHigh.Update(1);
            }
            else
            {
                PAccumBLevelHigh.Update(0);
            }

            // Accum B Level Low Safety Override
            if (wh01.SOAccumBLevelLowActuated)
            {
                PAccumBLevelLow.Update(1);
            }
            else
            {
                PAccumBLevelLow.Update(0);
            }

            if (wh01.SOAccumALevelHighActuated ||
                wh01.SOAccumALevelLowActuated ||
                wh01.SOAccumBLevelHighActuated ||
                wh01.SOAccumBLevelLowActuated)
            {
                if (wh01.PumpState)
                {
                    if (PumpOnBtn.IsEnabled) 
                        PumpOnBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                    
                else
                {
                    if (PumpOffBtn.IsEnabled)
                        PumpOffBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }

                if (wh01.DiverterValveState)
                {
                    if (DiverterValveAccumBBtn.IsEnabled)
                        DiverterValveAccumBBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }

                else
                {
                    if (DiverterValveAccumABtn.IsEnabled)
                        DiverterValveAccumABtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }

        private void UpdateShotclocks()
        {
            this.DSSTankLevelLow.Update(wh01.ttTankMassLow, !wh01.InValveState);
            this.DSSTankLevelHigh.Update(wh01.ttTankMassHigh, wh01.InValveState);
            this.DSSTankTempLow.Update(wh01.ttTankTempLow, !wh01.HeaterState);
            this.DSSTankTempHigh.Update(wh01.ttTankTempHigh, wh01.HeaterState);
            this.DSSTankAccumALow.Update(wh01.ttAccumAMassLow, !(wh01.PumpState & !wh01.DiverterValveState));
            this.DSSTankAccumAHigh.Update(wh01.ttAccumAMassHigh, wh01.PumpState);
            this.DSSTankAccumBLow.Update(wh01.ttAccumBMassLow, !(wh01.PumpState & wh01.DiverterValveState));
            this.DSSTankAccumBHigh.Update(wh01.ttAccumBMassHigh, wh01.PumpState);
        }

        private void PeriodicUpdateControls()
        {
            this.LevelIndicator.Update(wh01.TankLevel);
            if (wh01.TankLevel == 0.0)
            {
                this.TempIndicator.NoiseStd = TempSensorNoiseStd * 50.0;
            }
            else
            {
                this.TempIndicator.NoiseStd = TempSensorNoiseStd;
            }
            this.TempIndicator.Update(wh01.TempCur);
            this.TankInLabel.Content = String.Format("{0:0.0}", wh01.MinCur);
            this.TankInFLabel.Content = String.Format("{0:0.0}", Tin);
            this.ProcessAoutLabel.Content = String.Format("{0:0.0}", wh01.AccumAoutCur);
            this.ProcessBoutLabel.Content = String.Format("{0:0.0}", wh01.AccumBoutCur);
            this.AccumALevelIndicator.Update(wh01.AccumALevel);
            this.AccumBLevelIndicator.Update(wh01.AccumBLevel);
            this.TankOutLabel.Content = String.Format("{0:0.0}", wh01.MoutCur);
        }

        private void UpdateControls()
        {
            this.heater01.Update(wh01.HeaterState);
            this.valve01.Update(wh01.InValveState);
            this.divertervalve01.Update(wh01.DiverterValveState);
            this.pump.Update((wh01.PumpState && (wh01.MoutCur > 0)));
        }

        private IEnumerable<DSSControl> DSSControlEnumerator()
        {
            // AnnunciatorPanel is a Grid in the MainWindow
            foreach (var c in LogicalTreeHelper.GetChildren(DSSPanel))
                if (c is DSSControl)
                    yield return (c as DSSControl);
        }

        // DSS uses a hash to know whether it needs to update the DSS layout
        static UInt64 CalculateHash(string read)
        {
            UInt64 hashedValue = 3074457345618258791ul;
            for (int i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

        private UInt64 getDSSHash()
        {
            string hashstr = "t";
            foreach (DSSControl c in DSSList)
                hashstr += c.Action;

            return CalculateHash(hashstr);

        }

        private void UpdateDSSOrder()
        {
            
            foreach (DSSControl c in DSSControlEnumerator())
            {
                if (c.IsShowing)
                {
                    if (!DSSList.Any(item => item.Action == c.Action))
                        DSSList.Add(c);
                }
                else
                {
                    if (DSSList.Any(item => item.Action == c.Action))
                        DSSList.Remove(c);
                }
            }

            UInt64 newHash = getDSSHash();

            if (newHash != DSSHash)
            {
                int i = 0;
                foreach (DSSControl c in DSSList)
                {
                    Canvas.SetTop(c, i * 108);
                    i++;
                }
                DSSHash = newHash;
            }
        }

        private void WriteHeader()
        {
            // Write each directory name to a file. 
            using (StreamWriter sw = new StreamWriter(options.DataFile))
            {
                sw.WriteLine("time,massin,tank_level,temperature,massout," + 
                             "accumA_level,accumB_level,accumA_massout,accumB_massout," +
                             "valve_state,heater_state,pump_state,diverter_state," +
                             "valve_mode,heater_mode,pump_mode,diverter_mode," + 
                             "ttTankMassHigh,ttTankMassLow,ttTankTempHigh,ttTankTempLow,"+
                             "ttAccumAMassHigh,ttAccumAMassLow,ttAccumBMassHigh,ttAccumBMassLow");
            }
        }

        private void WriteLine()
        {
            // Write each directory name to a file. 
            using (StreamWriter sw = new StreamWriter(options.DataFile, true))
            {
                sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}," + 
                                           "{9},{10},{11},{12},{13},{14},{15},{16}," + 
                                           "{17},{18},{19},{20},{21},{22},{23},{24}", 
                                           t, 
                                           wh01.MinCur,
                                           wh01.TankLevel, 
                                           wh01.TempCur, 
                                           wh01.MoutCur, 
                                           wh01.AccumALevel,
                                           wh01.AccumBLevel,
                                           wh01.AccumAoutCur,
                                           wh01.AccumBoutCur,
                                           wh01.InValveState,
                                           wh01.HeaterState,
                                           wh01.PumpState,
                                           wh01.DiverterValveState,
                                           wh01.InValveMode,
                                           wh01.HeaterMode,
                                           wh01.PumpMode,
                                           wh01.DiverterValveMode,
                                           wh01.ttTankMassHigh,
                                           wh01.ttTankMassLow,
                                           wh01.ttTankTempHigh,
                                           wh01.ttTankTempLow,
                                           wh01.ttAccumAMassHigh,
                                           wh01.ttAccumAMassLow,
                                           wh01.ttAccumBMassHigh,
                                           wh01.ttAccumBMassLow));
            }
        }

        private void EndofTrialHndlr(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected virtual void Dispose(Boolean x)
        {
         
            player.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Dispose();
                Close();

            }
            else if (e.Key == Key.P)
            {
                player.PlayLooping();

            }
        }

        private void ShutdownHndlr(object sender, RoutedEventArgs e)
        {
            SCRAM();
        }

        public void SCRAM()
        {
            wh01.SCRAMed = true;
            
            ShutdownBtn.IsEnabled = false;
            
            InValveOpenBtn.IsEnabled = false;
            InValveAutoBtn.IsEnabled = false;
            InValveCloseBtn.IsEnabled = false;

            HeaterOnBtn.IsEnabled = false;
            HeaterAutoBtn.IsEnabled = false;
            HeaterOffBtn.IsEnabled = false;

            PumpOnBtn.IsEnabled = false;
            PumpAutoBtn.IsEnabled = false;
            PumpOffBtn.IsEnabled = false;

            DiverterValveAccumABtn.IsEnabled = false;
            DiverterValveAutoBtn.IsEnabled = false;
            DiverterValveAccumBBtn.IsEnabled = false;
        }

        private void TurnHeaterOnHndlr(object sender, RoutedEventArgs e)
        {
            HeaterOnBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            DSSControl c = (DSSControl)sender;
            c.DisregardBtn_Click(sender, new RoutedEventArgs(Button.ClickEvent));
        }

        private void TurnHeaterOffHndlr(object sender, RoutedEventArgs e)
        {
            HeaterOffBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            DSSControl c = (DSSControl)sender;
            c.DisregardBtn_Click(sender, new RoutedEventArgs(Button.ClickEvent));
        }

        private void HeaterHndlr(object sender, RoutedEventArgs e)
        {
            
            Button Btn = (Button)sender;
            String State = (String)Btn.Content;
            SetHeaterMode(State);
        }

        private void SetHeaterMode(String State)
        {
            wh01.HeaterMode = State;

            switch (State)
            {
                case "Auto":
                    HeaterOnBtn.IsEnabled = true;
                    HeaterAutoBtn.IsEnabled = false;
                    HeaterOffBtn.IsEnabled = true;
                    break;
                case "On":
                    HeaterOnBtn.IsEnabled = false;
                    HeaterAutoBtn.IsEnabled = true;
                    HeaterOffBtn.IsEnabled = true;
                    break;
                default:
                    HeaterOnBtn.IsEnabled = true;
                    HeaterAutoBtn.IsEnabled = true;
                    HeaterOffBtn.IsEnabled = false;
                    break;
            }

        }
        
        private void OpenFillValveHndlr(object sender, RoutedEventArgs e)
        {
            InValveOpenBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            DSSControl c = (DSSControl)sender;
            c.DisregardBtn_Click(sender, new RoutedEventArgs(Button.ClickEvent));
        }

        private void CloseFillValveHndlr(object sender, RoutedEventArgs e)
        {
            InValveCloseBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            DSSControl c = (DSSControl)sender;
            c.DisregardBtn_Click(sender, new RoutedEventArgs(Button.ClickEvent));
        }

        private void InValveHndlr(object sender, RoutedEventArgs e)
        {

            Button Btn = (Button)sender;
            String State = (String)Btn.Content;
            SetInValveMode(State);
        }

        private void SetInValveMode(String State)
        {
            wh01.InValveMode = State;

            switch (State)
            {
                case "Auto":
                    InValveOpenBtn.IsEnabled = true;
                    InValveAutoBtn.IsEnabled = false;
                    InValveCloseBtn.IsEnabled = true;
                    break;
                case "Open":
                    InValveOpenBtn.IsEnabled = false;
                    InValveAutoBtn.IsEnabled = true;
                    InValveCloseBtn.IsEnabled = true;
                    break;
                default:
                    InValveOpenBtn.IsEnabled = true;
                    InValveAutoBtn.IsEnabled = true;
                    InValveCloseBtn.IsEnabled = false;
                    break;
            }
        }

        private void TurnPumpOffHndlr(object sender, RoutedEventArgs e)
        {
            PumpOffBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            DSSControl c = (DSSControl)sender;
            c.DisregardBtn_Click(sender, new RoutedEventArgs(Button.ClickEvent));
        }
        
        private void DivertToAccumAHndlr(object sender, RoutedEventArgs e)
        {
            DiverterValveAccumABtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            PumpOnBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            DSSControl c = (DSSControl)sender;
            c.DisregardBtn_Click(sender, new RoutedEventArgs(Button.ClickEvent));
        }

        private void DivertToAccumBHndlr(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Print in DivertToAccumB");

            DiverterValveAccumBBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            PumpOnBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            DSSControl c = (DSSControl)sender;
            c.DisregardBtn_Click(sender, new RoutedEventArgs(Button.ClickEvent));
        }
        
        private void PumpHndlr(object sender, RoutedEventArgs e)
        {
            Button Btn = (Button)sender;
            String State = (String)Btn.Content;

            SetPumpMode(State);
        }

        public void SetPumpMode(String State)
        {
            wh01.PumpMode = State;

            switch (State)
            {
                case "Auto":
                    PumpOnBtn.IsEnabled = true;
                    PumpAutoBtn.IsEnabled = false;
                    PumpOffBtn.IsEnabled = true;
                    break;
                case "On":
                    PumpOnBtn.IsEnabled = false;
                    PumpAutoBtn.IsEnabled = true;
                    PumpOffBtn.IsEnabled = true;
                    break;
                default:
                    PumpOnBtn.IsEnabled = true;
                    PumpAutoBtn.IsEnabled = true;
                    PumpOffBtn.IsEnabled = false;
                    break;
            }

        }

        private void DiverterValveHndlr(object sender, RoutedEventArgs e)
        {
            Button Btn = (Button)sender;
            String State = (String)Btn.Content;
            SetDiverterValveMode(State);
        }

        private void SetDiverterValveMode(String State)
        {
            wh01.DiverterValveMode = State;

            switch (State)
            {
                case "Auto":
                    DiverterValveAccumABtn.IsEnabled = true;
                    DiverterValveAutoBtn.IsEnabled = false;
                    DiverterValveAccumBBtn.IsEnabled = true;
                    break;
                case "Accumulator A":
                    DiverterValveAccumABtn.IsEnabled = false;
                    DiverterValveAutoBtn.IsEnabled = true;
                    DiverterValveAccumBBtn.IsEnabled = true;
                    break;
                default:
                    DiverterValveAccumABtn.IsEnabled = true;
                    DiverterValveAutoBtn.IsEnabled = true;
                    DiverterValveAccumBBtn.IsEnabled = false;
                    break;
            }

        }

        private IEnumerable<Annunciator> AnnunciatorEnumerator()
        {
            // AnnunciatorPanel is a Grid in the MainWindow
            foreach (var c in LogicalTreeHelper.GetChildren(AnnunciatorPanel))
                if (c is Annunciator)
                    yield return (c as Annunciator);
        }

        private void TriggerAnnunciators()
        {
            foreach (Annunciator a in AnnunciatorEnumerator())
            {
                a.Reset();
                a.Trigger();
            }
        }

        private void SilenceAnnunciators()
        {
            foreach (Annunciator a in AnnunciatorEnumerator())
            {
                a.Silence();
            } 
            
            if (isPlaying)
            {
                isPlaying = false;
                player.Stop();
            }
        }

        private void AcknowledgeAnnunciators()
        {
            foreach (Annunciator a in AnnunciatorEnumerator())
            {
                a.Acknowledge();
            }

            if (isPlaying)
            {
                isPlaying = false;
                player.Stop();
            }

        }

        private void ResetAnnunciators()
        {
            foreach (Annunciator a in AnnunciatorEnumerator())
            {
                a.Reset();
            }
        }

        private void AnnunciatorHndlr(object sender, RoutedEventArgs e)
        {
            Button Btn = (Button)sender;
            String State = (String)Btn.Content;

            switch (State)
            {
                case "Test":
                    TriggerAnnunciators();
                    break;
                case "Silence":
                    SilenceAnnunciators();
                    break;
                case "Ack.":
                    AcknowledgeAnnunciators();
                    break;
                case "Reset":
                    ResetAnnunciators();
                    break;
            }
        }

        private void AccumAOutFlowSldr_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            AccumAout = AccumAOutFlowSldr.Value;
        }

        private void AccumBOutFlowSldr_ValueChanged(object sender, 
            RoutedPropertyChangedEventArgs<double> e)
        {
            AccumBout = AccumBOutFlowSldr.Value;
        }

        private void InFlowSldr_ValueChanged(object sender, 
            RoutedPropertyChangedEventArgs<double> e)
        {
            Mavailable = InFlowSldr.Value;
        }

        private void TinSldr_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            Tin = TinSldr.Value;
        }

        private void TankOutFlowSldr_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            TankOutFlow = TankOutFlowSldr.Value;
        }
    }
}
