using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace proftaak
{
    public class Conn
    {
        bool isConnected = false;
        String[] ports;
        SerialPort port;
         
        public void connectToArduino(string selectedPort)
        {
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);

            try
            {
                port.Open();

                isConnected = true;

                // Write to serial monitor
                port.Write("100\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }

            
        }
    }
}
