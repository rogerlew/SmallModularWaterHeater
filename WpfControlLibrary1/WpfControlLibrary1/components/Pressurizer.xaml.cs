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
    /// Interaction logic for Pressurizer.xaml
    /// </summary>
    public partial class Pressurizer : UserControl
    {
        public Pressurizer()
        {
            InitializeComponent();
        }

        public void UpdateLevel(double percentageLevel)
        {
            LevelRect.Height = percentageLevel / 100.0 * 272.0;
        }
    }
}
