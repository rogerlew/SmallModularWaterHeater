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
    /// Interaction logic for SteamGenerator.xaml
    /// </summary>
    public partial class SteamGenerator : UserControl
    {
        public SteamGenerator()
        {
            InitializeComponent();
        }

        public void UpdateLevel(double percentageLevel)
        {
            LevelRect.Height = percentageLevel / 100.0 * 250.0;
            if (percentageLevel < 68.0 / 2.5)
            {
                LevelRect2.Height = 0.0;
            }
            else if (percentageLevel >= 68.0 / 2.5 & percentageLevel < 82.0 / 2.5)
            {
                LevelRect2.Height = (percentageLevel - 68.0 / 2.5) * 2.5;
            }
            else
            {
                LevelRect2.Height = 14.0;
            }
        }
    }
}
