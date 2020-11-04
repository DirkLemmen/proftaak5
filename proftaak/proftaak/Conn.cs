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
        public bool isConnected = false;

        public bool isBlinking = false;

        SerialPort port;

        static int old_intensity = 0;

       

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

        public void lightReset()
        {
            if (isConnected)
            {
                port.Write("-1\n");
            }
        }

        public void changeIntensity(int intensity)
        {
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

        public void lightBlink()
        {
            if (isConnected)
            {
                if (!isBlinking)
                {
                    port.Write("#BLNK\n");
                    isBlinking = true;
                }
                else
                {
                    port.Write("!BLNK\n");
                }
            }
        }
    }
}
