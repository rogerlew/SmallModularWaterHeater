using System;
using System.Collections.Generic;
using System.IO;
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

using MahApps.Metro.Controls;
using System.Diagnostics;

namespace SMWH_Instructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : MetroWindow
    {
        bool initialized = false;

        public MainWindow()
        {
            InitializeComponent();

            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2.0 - this.Width / 2.0;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2.0 - this.Height / 2.0;

            initialized = true;
        }

        private bool hasPID()
        {
            return (bool)(tbPID.Text.Length > 0);
        }

        private bool hasLOA()
        {
            return (bool)(getLOA().Length > 0);
        }

        private String getLOA()
        {
            if (cbLOA.SelectedValue == null)
                return "";

            ComboBoxItem foo = (ComboBoxItem)(cbLOA.SelectedValue);
            return foo.Content.ToString();
        }

        private String buildFileName()
        {
            return  String.Format("./Data/{0}-{1}-SMWH00.csv", tbPID.Text, getLOA());
            
        }

        private void CheckLaunchReady(object sender, TextChangedEventArgs e)
        {
            CheckLaunchReady();
        }

        private void CheckLaunchReady(object sender, SelectionChangedEventArgs e)
        {
            CheckLaunchReady();
        }

        private void CheckLaunchReady()
        {
            if (!initialized)
                return;

            if (hasPID() && hasLOA())
            {
                btnLaunch.IsEnabled = true;

                String fname = buildFileName();
                lblDataFile.Content = fname;

                if (File.Exists(fname))
                    lblOverwrite.Content = "Warning: Datafile already exists!\n              Data will be overwritten.";
                else
                    lblOverwrite.Content = "";
            }
            else
            {
                btnLaunch.IsEnabled = false;
                lblDataFile.Content = "";
                lblOverwrite.Content = "";
            }
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo.FileName = @".\WaterHeater.exe";
            p.StartInfo.Arguments = "-l" + getLOA() + ".ini" + 
                                    " -t" + tbRuntime.Text +
                                    " -d" + buildFileName();
            p.Start();
         
            Close();
        }


    }
}
