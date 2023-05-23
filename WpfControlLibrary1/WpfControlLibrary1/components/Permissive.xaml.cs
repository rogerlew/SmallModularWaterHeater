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
    /// Interaction logic for Permissive.xaml
    /// </summary>
    public partial class Permissive : UserControl
    {

        // Title <string>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
                                        typeof(Permissive));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public Permissive()
        {
            InitializeComponent();
        }

        public void Update(double Status)
        {
            if (Status == 0)
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));
            }
            else
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
        }
    }
}
