using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using CsvHelper;

namespace Sensors
{
    class Process
    {

    }

    class ArProcess
    {
        public bool ar_process_shutdown = false;
        string ar_data;
        string parsed_ar_data;
        string part_of_path = System.IO.Directory.GetCurrentDirectory();
        string path;

        public void ProcessArData()
        {
            Arduino arduino = new Arduino();
            while(!arduino.FindArPort())
            {
                Thread.Sleep(10000);
            }

            while (!ar_process_shutdown)
            {
                ar_data = arduino.ReadArData();
                ArduinoData parsed_ar_data = JsonConvert.DeserializeObject<ArduinoData>(ar_data);
                List<ArduinoData> data = parsed_ar_data;
                DateTime thisDay = DateTime.Today;
                path = part_of_path + @"\" + thisDay.Year.ToString() + @"\" + thisDay.Month.ToString();
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                StreamWriter writer = new StreamWriter(path + @"\" + thisDay.Day.ToString() + ".csv");
                CsvWriter csv = new CsvWriter(writer);
                
            }
        }
    }
}
