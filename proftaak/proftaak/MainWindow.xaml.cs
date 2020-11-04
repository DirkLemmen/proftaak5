using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Threading;

namespace proftaak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Conn conn = new Conn();

        public MainWindow()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                cbPorts.Items.Add(port);

                if (ports[0] != null)
                {
                    cbPorts.SelectedItem = ports[0];
                }
            }

        }

        private void cbPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get selected port
            string selectedPort = cbPorts.SelectedItem.ToString();
            // Try connecting
                conn.connectToArduino(selectedPort);

            if (conn.isConnected == true)
            {
                enableControls();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            // Disable manual light
            conn.lightReset();
        }

        DateTime oldStamp = DateTime.Now;
        private void sldIntensity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Delay with 100 MS to prevent sending to much bytes to Arduino  
         
            DateTime dt = DateTime.Now;

            TimeSpan ms = dt.Subtract(oldStamp);


            if (ms.TotalMilliseconds > 100)
            {
                // Change intensity
                conn.changeIntensity(Convert.ToInt32(sldIntensity.Value));
            }

           
            oldStamp = dt;
        }

        private void disableControles()
        {
            btnReset.IsEnabled = false;
            sldIntensity.IsEnabled = false;
            btnBlink.IsEnabled = false;
        }

        private void enableControls()
        {
            btnReset.IsEnabled = true;
            sldIntensity.IsEnabled = true;
            btnBlink.IsEnabled = true;
        }

        private void btnBlink_Click(object sender, RoutedEventArgs e)
        {
            conn.lightBlink();
        }
    }
}
