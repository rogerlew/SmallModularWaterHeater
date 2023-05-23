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
	/// Interaction logic for ControlValve.xaml
	/// </summary>
	public partial class ControlValve : UserControl
	{
		public ControlValve()
		{
			this.InitializeComponent();
            State = 0.0;
        }
        public double State
        {
            get;
            set;
        }

        // Text <string>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                                        typeof(ControlValve));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public void Update(double State)
        {
            this.State = State;

            if (this.State == 0.0)
            {
                this.Bowtie.Style = (Style)this.Resources["PolygonStyle"];
            }
            else
            {
                this.Bowtie.Style = (Style)this.Resources["PolygonStyleK"];
            }

        }

        public void Update()
        {
            Update(Convert.ToDouble(Text));

        }
    }
}