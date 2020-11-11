using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace proftaak
{
    public class Conn
    {
        public bool isConnected = false;

        public bool isBlinking = false;

        SerialPort port;

        static int old_intensity = 0;

       
        /// <summary>
        /// Try connecting to Arduino trough port
        /// </summary>
        /// <param name="selectedPort"></param>
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

        /// <summary>
        /// Reset light 
        /// </summary>
        public void lightReset()
        {
            try
            {
                if (isConnected)
                {
                    port.Write("-1\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
            
        }

        /// <summary>
        /// Change the intensity of the light
        /// </summary>
        /// <param name="intensity"></param>
        public void changeIntensity(int intensity)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
            
        }

        /// <summary>
        /// Blink light
        /// </summary>
        public void lightBlink()
        {
            try
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
                        isBlinking = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }  
        }
    }
}
