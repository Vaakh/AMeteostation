﻿using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Timers;

namespace Connections
{
    class SerialConnection
    {

        int serial_speed = 9600;
        string arduino_message;
        System.Timers.Timer aTimer;
        SerialPort main_serial_port;

        private bool ArduinoDetected()
        {
            try
            {
                main_serial_port.Open();
                System.Threading.Thread.Sleep(10); // just wait a lot
                arduino_message = main_serial_port.ReadExisting();
                //Console.WriteLine(ArduinoMessage);
                main_serial_port.Close();
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
                main_serial_port.Open();
                System.Threading.Thread.Sleep(10);
                arduino_message = main_serial_port.ReadLine();
                main_serial_port.Close();
                return arduino_message;
            }
            catch
            {
                FindArPort();
                if (ArduinoDetected())
                {
                    return ("Connection sucsessfully!");
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
                    main_serial_port = new SerialPort(port, serial_speed);
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

    }

    class SerialExecutor
    {
        public static void Executor()
        {
            SerialConnection connection = new SerialConnection();
            connection.FindArPort();
            string data;
            Console.WriteLine("Start!");
            while (true)
            {
                data = connection.ReadArData();
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

