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
	/// Interaction logic for Tank.xaml
	/// </summary>
	public partial class Tank : UserControl
	{
		public Tank()
		{
			this.InitializeComponent();
		}
        
        public void UpdateLevel(double percentageLevel)
        {
            LevelRect.Height = percentageLevel / 100.0 * 282.0;
        }
	}
}