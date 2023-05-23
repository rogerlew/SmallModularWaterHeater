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

namespace WpfControlLibrary1
{
    /// <summary>
    /// Interaction logic for ValveTest.xaml
    /// </summary>
    public partial class ValveTest : UserControl
    {
        public double valvedemanddouble;
        public double valveactualdouble;
        public ValveTest()
        {
            InitializeComponent();
        }
        #region DependencyProperty Declarations
        // Units <string>
        public static DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(string),
                                        typeof(ValveTest));

        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        // ValueActualStr <string>
        public static DependencyProperty ValueActualProperty =
            DependencyProperty.Register("ValueActual", typeof(string),
                                        typeof(ValveTest));

        public string ValueActual
        {
            get { return (string)GetValue(ValueActualProperty); }
            set
            {
                SetValue(ValueActualProperty, value);
            }
        }
        // ValueStr <string>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string),
                                        typeof(ValveTest));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        // ValueDemandStr <string>
        public static DependencyProperty ValueDemandProperty =
            DependencyProperty.Register("ValueDemand", typeof(string),
                                        typeof(ValveTest));

        public string ValueDemand
        {
            get { return (string)GetValue(ValueDemandProperty); }
            set
            {
                SetValue(ValueDemandProperty, value);
            }
        }

        // TitleStr <string>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
                                        typeof(ValveTest));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        #endregion

        public void Update()
        {
            valveactualdouble = Convert.ToDouble(ValueActual);
            double heightactual = actualcontainer.ActualHeight;
            baractual.Height = valveactualdouble * heightactual;
            baractual.Margin = new Thickness(0, 200 - baractual.Height, 0, 0);
            Value = (valveactualdouble * 100).ToString();

            valvedemanddouble = Convert.ToDouble(ValueDemand);
            double heightdemand = demandcontainer.ActualHeight;
            bardemand.Height = valvedemanddouble/100 * heightdemand;
            bardemand.Margin = new Thickness(0,200 - bardemand.Height,0,0);
        }





    }
}
