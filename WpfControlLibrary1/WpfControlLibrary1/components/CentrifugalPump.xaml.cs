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
	/// Interaction logic for CentrifugalPump.xaml
	/// </summary>
	public partial class CentrifugalPump : UserControl
	{
		public CentrifugalPump()
		{
			this.InitializeComponent();
            State = false;
            operationalState = true;
        }
        public bool State
        {
            get;
            set;
        }
        
        public bool operationalState
        {
            get;
            set;
        }

        public void Update(bool State)
        {
            this.State = State;

            if (this.State)
            {
                this.Outlet.Style = (Style)this.Resources["RectangleStyleK"];
                this.Body.Style = (Style)this.Resources["EllipseStyleK"];
                this.Inlet.Style = (Style)this.Resources["EllipseStyleK"];
            }
            else
            {
                this.Outlet.Style = (Style)this.Resources["RectangleStyle"];
                this.Body.Style = (Style)this.Resources["EllipseStyle"];
                this.Inlet.Style = (Style)this.Resources["EllipseStyle"];
            }



            if (operationalState)
            {
                Crack1.Visibility = Visibility.Hidden;
                Crack2.Visibility = Visibility.Hidden;
            }
            else
            {
                Crack1.Visibility = Visibility.Visible;
                Crack2.Visibility = Visibility.Visible;
            }
        }
	}
}