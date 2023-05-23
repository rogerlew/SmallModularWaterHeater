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
	/// Interaction logic for DiverterValve.xaml
	/// </summary>
	public partial class DiverterValve : UserControl
	{
		public DiverterValve()
		{
			this.InitializeComponent();
            State = false;
        }
        public bool State
        {
            get;
            set;
        }

        public void Update(bool State)
        {
            this.State = State;

            if (this.State)
            {
                this.Left.Style = (Style)this.Resources["PolygonStyleK"];
                this.Right.Style = (Style)this.Resources["PolygonStyle"];
            }
            else
            {
                this.Right.Style = (Style)this.Resources["PolygonStyleK"];
                this.Left.Style = (Style)this.Resources["PolygonStyle"];
            }

        }
	}
}