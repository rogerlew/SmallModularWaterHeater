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
    /// Interaction logic for Heater.xaml
    /// </summary>
    public partial class Heater : UserControl
    {
        public Heater()
        {
            InitializeComponent();
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
                this.Element.Stroke = Brushes.Black;
            }
            else
            {
                this.Element.Stroke = Brushes.White;
            }

        }
    }
}
