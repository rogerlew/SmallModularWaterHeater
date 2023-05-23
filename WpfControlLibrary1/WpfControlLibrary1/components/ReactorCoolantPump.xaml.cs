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
    /// Interaction logic for ReactorCoolantPump.xaml
    /// </summary>
    public partial class ReactorCoolantPump : UserControl
    {
        public ReactorCoolantPump()
        {
            this.InitializeComponent();
            State = false;
        }
        public bool State
        {
            get;
            set;
        }


        // Text <string>
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                                        typeof(ReactorCoolantPump));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public void Update(bool State)
        {
            this.State = State;

            if (this.State)
            {
                this.Shaft.Style = (Style)this.Resources["RectangleStyleK"];
                this.Inlet1.Style = (Style)this.Resources["RectangleStyleK"];
                this.Inlet2.Style = (Style)this.Resources["RectangleStyleK"];
                this.Body.Style = (Style)this.Resources["RectangleStyleK"];
            }
            else
            {
                this.Shaft.Style = (Style)this.Resources["RectangleStyle"];
                this.Inlet1.Style = (Style)this.Resources["RectangleStyle"];
                this.Inlet2.Style = (Style)this.Resources["RectangleStyle"];
                this.Body.Style = (Style)this.Resources["RectangleStyle"];
            }

        }

        public void Update(double State)
        {
            this.State = (State == 1.0);
            Update(this.State);
        }

        public void Update()
        {
            Update(Convert.ToDouble(Text));
        }

    }
}