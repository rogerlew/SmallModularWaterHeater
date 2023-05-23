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
    /// Interaction logic for StopButton.xaml
    /// </summary>
    public partial class StopButton : UserControl
    {
        // Fill <string>
        public static DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(SolidColorBrush),
                                        typeof(StopButton),
                                        new PropertyMetadata(Brushes.Black));

        public SolidColorBrush Fill
        {
            get { return (SolidColorBrush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public StopButton()
        {
            InitializeComponent();
        }
    }
}
