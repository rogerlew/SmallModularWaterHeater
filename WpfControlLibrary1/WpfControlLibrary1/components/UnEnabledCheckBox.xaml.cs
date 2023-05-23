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
    /// Interaction logic for UnEnabledCheckBox.xaml
    /// </summary>
    public partial class UnEnabledCheckBox : UserControl
    {
        // IsChecked <bool>
        public static DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool),
                                        typeof(UnEnabledCheckBox),
                                        new PropertyMetadata(false));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }


        public UnEnabledCheckBox()
        {
            InitializeComponent();
        }
    }
}
