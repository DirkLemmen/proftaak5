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

namespace proftaak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isConnected = false;
        String[] ports;
        SerialPort port;

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
            if (!isConnected)
            {
                connectToArduino();
            }
            else
            {
                disconnectFromArduino();
            }
        }

        private void connectToArduino()
        {
            isConnected = true;
            string selectedPort = cbPorts.SelectedItem.ToString();

            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            port.Write("100\n");
        }

        private void disconnectFromArduino()
        {
            isConnected = false;
            port.Write("#STOP\n");
            port.Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                port.Write("-1\n");
            }
        }
        static int old_intensity = 0;

        private void sldIntensity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int intensity = Convert.ToInt32(sldIntensity.Value);
            if (intensity == old_intensity)
            {
                return;
            }

            if (isConnected)
            {
                port.Write(intensity + "\n");
                Console.WriteLine(intensity);
                old_intensity = intensity;
            }
        }

    }
}
