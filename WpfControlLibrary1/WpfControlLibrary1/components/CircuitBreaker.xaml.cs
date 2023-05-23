﻿using System;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CircuitBreaker : UserControl
    {
        public CircuitBreaker()
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
                                        typeof(CircuitBreaker));

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
                this.Breaker.Stroke = Brushes.Black;
            }
            else
            {
                this.Breaker.Stroke = Brushes.White;
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
