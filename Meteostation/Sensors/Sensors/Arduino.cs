using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Timers;


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

        public string ReadAllArData()
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

        public void FindArPort()
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
            }
            catch { Console.WriteLine("FindArPort failed!"); }
            if (ArduinoPortFound == false) FindArPort();
        }

        public bool ProcessAllData()
        {
            string data;

            data = ReadAllArData();
            if (data.Contains("Connection failed!"))
            {
                return false;
            }
            if (data.Contains("Data from Arduino"))
            {
                Console.WriteLine(data);
                return true;
            }
            data = "Wrong data";
            return false;
        }

        public bool Initialize()
        {
            return true;
        }
    }

    class ArduinoExecutor
    {

        public static bool Initialze()
        {
            return true;
        }


        public static void ReadAllData()
        {
            Arduino connection = new Arduino();
            connection.FindArPort();
            string data;
            Console.WriteLine("Start!");
            while (true)
            {
                data = connection.ReadAllArData();
                if (data.Contains("Connection failed!"))
                {
                    connection.FindArPort();
                }
                if (data.Contains("Data from Arduino"))
                {
                    Console.WriteLine(data);
                }
                System.Threading.Thread.Sleep(100); // just wait a lot

            }
        }
    }
}
