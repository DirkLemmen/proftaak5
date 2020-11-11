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
using System.Windows.Threading;

namespace proftaak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Conn conn = new Conn();

        int startTime;
        int endTime;
        int currentTime;

        public MainWindow()
        {
            InitializeComponent();

            // Laad poorten in combobox
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                cbPorts.Items.Add(port);

                if (ports[0] != null)
                {
                    cbPorts.SelectedItem = ports[0];
                }
            }

            // Timer setup
            DispatcherTimer dt = new DispatcherTimer();
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0,0,1);
            dt.Start();

        }

        private void cbPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get selected port
            string selectedPort = cbPorts.SelectedItem.ToString();
            // Try connecting
                conn.connectToArduino(selectedPort);

            if (conn.isConnected == true)
            {
                // code
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

        private void btnBlink_Click(object sender, RoutedEventArgs e)
        {
            conn.lightBlink();
        }

        private void tpStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Get start time
            DateTime? dtStart = tpStart.Value;
            if (dtStart.HasValue)
            {
                startTime = (int)dtStart.Value.TimeOfDay.TotalMinutes;
            }

        }

        private void tpEnd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Get end time
            DateTime? dtEnd = tpEnd.Value;
            if (dtEnd.HasValue)
            {
                endTime =  (int)dtEnd.Value.TimeOfDay.TotalMinutes;
            }
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            // Get current time
            currentTime = (int)Math.Floor(DateTime.Now.TimeOfDay.TotalMinutes);

            // Turn light on or off depending on time
            if (startTime == currentTime)
            {
                conn.lightReset();
                Console.WriteLine("on");
            }
            if (endTime == currentTime)
            {
                conn.changeIntensity(0);
                Console.WriteLine("off");

            }
        }
    }
}
