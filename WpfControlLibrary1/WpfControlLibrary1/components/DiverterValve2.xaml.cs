using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for DiverterValve2.xaml
    /// </summary>
    public partial class DiverterValve2 : UserControl
    {
        public DiverterValve2()
        {
			this.InitializeComponent();
            State = 0.5;
        }
        public double State
        {
            get;
            set;
        }

        // Text <string>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                                        typeof(DiverterValve2));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public void Update(double State)
        {
            this.State = State;

            if (this.State == 1.0)
            {
                this.Left.Style = (Style)this.Resources["PolygonStyleK"];
                this.Down.Style = (Style)this.Resources["PolygonStyle"];
            }
            else if (this.State == 0.0)
            {
                this.Down.Style = (Style)this.Resources["PolygonStyleK"];
                this.Left.Style = (Style)this.Resources["PolygonStyle"];
            }
            else
            {
                this.Down.Style = (Style)this.Resources["PolygonStyle"];
                this.Left.Style = (Style)this.Resources["PolygonStyle"];
            }

        }

        public void Update()
        {
            Update(Convert.ToDouble(Text));

        }
    }
}
