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
    /// Interaction logic for Close.xaml
    /// </summary>
    public partial class Close : UserControl
    {
        public Close()
        {
            InitializeComponent();
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            //SolidColorBrush newBrush2 = new SolidColorBrush();
            //newBrush2.Color = Color.FromRgb((byte)248, (byte)248, (byte)248);
            //CanvasBackground.Background = newBrush2;
        }
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
        //    CanvasBackground.Background = Brushes.Gray;
        }
    }
}
