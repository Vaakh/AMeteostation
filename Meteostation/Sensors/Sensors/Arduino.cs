using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Timers;
using CsvHelper;

namespace Sensors
{
    class Arduino
    {

        int serial_speed = 9600;
        string arduino_message;
        System.Timers.Timer aTimer;
        SerialPort arduino_port;

        private bool ArduinoDetected()
        {
            try
            {
                arduino_port.Open();
                System.Threading.Thread.Sleep(10); // just wait a lot
                arduino_message = arduino_port.ReadExisting();
                //Console.WriteLine(ArduinoMessage);
                arduino_port.Close();
                // in arduino sketch should be Serial.println("Data from Arduino") inside  void loop()
                if (arduino_message.Contains("Data from Arduino"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Console.WriteLine("ArduinoDetected failed");
                return false;
            }
        }

        public string ReadArData()
        {
            try
            {
                arduino_port.Open();
                System.Threading.Thread.Sleep(10);
                arduino_message = arduino_port.ReadLine();
                arduino_port.Close();
                return arduino_message;
            }
            catch
            {
                FindArPort();
                if (ArduinoDetected())
                {
                    return ("ReConnection sucsessfully!");
                }
                else return ("Connection failed!");
            }
        }

        public bool FindArPort() //actually, initialization
        {
            bool ArduinoPortFound = false;
            // this method contains recursion
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    arduino_port = new SerialPort(port, serial_speed);
                    if (ArduinoDetected())
                    {
                        ArduinoPortFound = true;
                        Console.WriteLine(port);
                        break;
                    }
                    else
                    {
                        //System.Threading.Thread.Sleep(1000);
                        ArduinoPortFound = false;
                    }
                }
                if (ArduinoPortFound == true) return true;
                else return false;
            }
            catch { Console.WriteLine("FindArPort failed!"); return false; }
        }

    }

    class ArduinoData
    {
        public string[] RTC = new string[2]; //(RealTime Clock) first element date, second - time
        public int[] MFS = new int[3]; //(Magnit Field Sensor) standart sequence: x, y, z
        public int[] THI = new int[2]; //(TempIns/HumIns) first temp, second - hum
        public int[] THO = new int[2]; //(TempOut/HumOut) first temp, second - hum
        public int[] TPO = new int[2]; //(TempOut/PressOut) first temp, second - press
        public int[] Wnd = new int[2]; //(WindSpeed/WindOrintation) first speed, second - orientation
        public int LLS; //(LightLevel Sensor)
        public int RDS; //(RainDrop Sensor)
        public int IRS; //(Infrored Sensor)

    }

}
