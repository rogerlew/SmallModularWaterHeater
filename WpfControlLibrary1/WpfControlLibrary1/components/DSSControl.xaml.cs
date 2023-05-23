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
    /// Interaction logic for DSSControl.xaml
    /// </summary>
    public partial class DSSControl : UserControl
    {

        #region Title <string>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
                                        typeof(DSSControl));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        #endregion

        #region Action <string>
        public static DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(string),
                                        typeof(DSSControl));

        public string Action
        {
            get { return (string)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }
        #endregion

        #region ShotClock <string>
        public static DependencyProperty ShotClockProperty =
            DependencyProperty.Register("ShotClock", typeof(string),
                                        typeof(DSSControl));

        public string ShotClock
        {
            get { return (string)GetValue(ShotClockProperty); }
            set { SetValue(ShotClockProperty, value); }
        }
        #endregion

        #region TimeToEvent <string>
        public static DependencyProperty TimeToEventProperty =
            DependencyProperty.Register("TimeToEvent", typeof(double),
                                        typeof(DSSControl));

        public double TimeToEvent
        {
            get { return (double)GetValue(TimeToEventProperty); }
            set { SetValue(TimeToEventProperty, value); }
        }
        #endregion

        #region Disregard <bool>
        public static DependencyProperty DisregardProperty =
            DependencyProperty.Register("Disregard", typeof(bool),
                                        typeof(DSSControl));

        public bool Disregard
        {
            get { return (bool)GetValue(DisregardProperty); }
            set { SetValue(DisregardProperty, value); }
        }
        #endregion


        #region CanAct <bool>
        public static DependencyProperty CanActProperty =
            DependencyProperty.Register("CanAct", typeof(bool),
                                        typeof(DSSControl));

        public bool CanAct
        {
            get { return (bool)GetValue(CanActProperty); }
            set { SetValue(CanActProperty, value); }
        }
        #endregion


        #region WarningTime <double>
        public static DependencyProperty WarningTimeProperty =
            DependencyProperty.Register("WarningTime", typeof(double),
                                        typeof(DSSControl),
                                        new PropertyMetadata(5.0));

        public double WarningTime
        {
            get { return (double)GetValue(WarningTimeProperty); }
            set { SetValue(WarningTimeProperty, value); }
        }
        #endregion

        #region IsShowing <bool>
        public static DependencyProperty IsShowingProperty =
            DependencyProperty.Register("IsShowing", typeof(bool),
                                        typeof(DSSControl),
                                        new UIPropertyMetadata(true));

        public bool IsShowing
        {
            get { return (bool)GetValue(IsShowingProperty); }
            set { SetValue(IsShowingProperty, value); }
        }
        #endregion

        public DSSControl()
        {
            InitializeComponent();
        }


        public void Update(double Value, bool CanAct)
        {
            this.CanAct = CanAct;
            bool newShow = false;

            TimeToEvent = Value;
            if (CanAct && !Disregard && (TimeToEvent <= WarningTime))
            {
                newShow = true;
                ShotClock = String.Format("{0:0.0}", Value);
            }
            else
            {
                ShotClock = "";
            }

            if (newShow != IsShowing)
            {
                if (newShow)
                    Show();
                else
                    Hide();
            }

        }

        public void Show()
        {
            IsShowing = true;
            DoubleAnimation db = new DoubleAnimation();
            db.From = 0;
            db.To = 1;
            db.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            this.BeginAnimation(OpacityProperty, db);
                
        }
        public void Hide()
        {
            DoubleAnimation db = new DoubleAnimation();
            db.From = 1;
            db.To = 0;
            db.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            this.BeginAnimation(OpacityProperty, db);
            Canvas.SetTop(this, 600.0);
            IsShowing = false;
        }

        public void DisregardBtn_Click(object sender, RoutedEventArgs e)
        {
            Disregard = true;

            System.Windows.Threading.DispatcherTimer dispatcherTimer = 
                new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0,0,0,3);
            dispatcherTimer.Start();
            e.Handled = true;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Disregard = false;
        }


        // Button routing based on
        // http://chuckhays.net/blog/2010/05/21/very-simple-routed-event-example/

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DSSControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("handling RunBtn_Click");
            this.RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            e.Handled = true;
        }

    }
}
