using System;
using System.IO.Ports;
using System.Timers;

namespace Sensors
{
    class Arduino
    {

        int serial_speed = 9600;
        string arduino_message;
        Timer aTimer;
        SerialPort arduino_port;

        private bool ArduinoDetected()
        {
            try
            {
                arduino_port.Open();
                System.Threading.Thread.Sleep(10); // just wait a lot
                arduino_message = arduino_port.ReadLine();
                Console.WriteLine(arduino_message + " - port message");
                arduino_port.Close();
                // in arduino sketch should be ("Data_From_Arduino") inside  void loop()
                if (arduino_message.Contains("Data_From_Arduino"))
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
        public int Data_From_Arduino; // ID
        public string RTC_date; // RealTime Clock Date
        public string RTC_time; // RealTime Clock Time
        public float MFS_x; // Magnit Field Sensor 
        public float MFS_y; // Magnit Field Sensor
        public float MFS_z; // Magnit Field Sensor
        public float THI_t; // Temperature Inside
        public float THI_h; // Humidity Inside
        public float THO_t; // Temperature Outside
        public float THO_h; // Humidity Outside
        public float TPO_t; // Temperature Outside
        public float TPO_p; // Pressure Outside
        public float WND_s; // Wind Speed 
        public float WND_o; // Wind Orintation
        public int LLS; // LightLevel Sensor
        public int RDS; // RainDrop Sensor
        public float IRS; // Infrored Sensor

    }

}
