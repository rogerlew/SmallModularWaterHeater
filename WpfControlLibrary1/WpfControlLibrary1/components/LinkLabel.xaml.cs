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
    /// Interaction logic for Link.xaml
    /// </summary>
    public partial class LinkLabel : UserControl
    {

        // LabelContent <string>
        public static DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(string),
                                        typeof(LinkLabel));

        public string LabelContent
        {
            get { return (string)GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }


        public LinkLabel()
        {
            InitializeComponent();
        }
    }
}
