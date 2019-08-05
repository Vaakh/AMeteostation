using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using CsvHelper;
using MySql.Data.MySqlClient;

namespace Sensors
{
    class Process
    {

        public bool isTableExists(string tableName, MySqlConnection conn)
        {
            bool exists;

            try
            {
                // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
                MySqlCommand cmd = new MySqlCommand(
                 "(select * from information_schema.tables where table_name = '" + tableName + "')", conn);
                string compareName = cmd.ExecuteScalar().ToString();
                exists = true;
                return exists;
            }
            catch
            {
                try
                {
                    // Other RDBMS.  Graceful degradation
                    exists = true;
                    var cmdOthers = new MySqlCommand("select 1 from " + tableName + " where 1 = 0", conn);
                    cmdOthers.ExecuteNonQuery();
                }
                catch
                {
                    exists = false;
                    return exists;
                }
            }

            return exists;
        }

    }

    class ArProcess
    {
        public bool ar_process_shutdown = false;
        string ar_data;

        public void ProcessArData()
        {
            try
            {
                Process arProc = new Process();
                Arduino arduino = new Arduino();
                
                while (!arduino.FindArPort())
                {
                    Thread.Sleep(1000);
                }
                Console.WriteLine("ArduinoPort found");
                while (!ar_process_shutdown)
                {
                    ar_data = arduino.ReadArData();
                    ArduinoData p_data = JsonConvert.DeserializeObject<ArduinoData>(ar_data);
                    Console.WriteLine("Flag 1");
                    DateTime thisDay = DateTime.Today;
                    string part_con = "server=localhost;user=Meteostation;database=datatest;password=OpenSourse!123"; 
                    MySqlConnection conn = new MySqlConnection(part_con);
                    conn.Open();
                    string qery;
                    Console.WriteLine("Flag 2");
                    //string tableName = thisDay.Year.ToString() + "/" + thisDay.Month.ToString() + "/" + thisDay.Day.ToString();
                    string tableName = p_data.RTC_date;

                    bool exists = arProc.isTableExists(tableName, conn);
                    Console.WriteLine(p_data.MFS_x);
                    if (exists == false)
                    {
                        qery = "CREATE TABLE `datatest`.`" + tableName + "` ( `RTC_time` TIME NOT NULL, `MFS_x` FLOAT NULL, `MFS_y` FLOAT NULL, `MFS_z` FLOAT NULL, `THI_t` DOUBLE NULL, `THI_h` FLOAT NULL, `THO_t` FLOAT NULL, `THO_h` FLOAT NULL, `TPO_t` FLOAT NULL, `TPO_p` FLOAT NULL, `WND_s` FLOAT NULL, `WND_o` FLOAT NULL, `LLS` INT NULL, `RDS` INT NULL, `IRS` FLOAT NULL, `photo_path` VARCHAR(120) NULL, PRIMARY KEY(`RTC_time`), UNIQUE INDEX `RTC_time_UNIQUE` (`RTC_time` ASC) VISIBLE);";
                        MySqlCommand createTable = new MySqlCommand(qery, conn);
                        createTable.ExecuteNonQuery();
                    }

                    qery = "INSERT INTO `datatest`.`" + tableName + "` (`RTC_time`, `MFS_x`, `MFS_y`, `MFS_z`, `THI_t`, `THI_h`, `THO_t`, `THO_h`, `TPO_t`, `TPO_p`, `WND_s`, `WND_o`, `LLS`, `RDS`, `IRS`) VALUES('" + p_data.RTC_time + "', '" + dataPrep(p_data.MFS_x) + "', '" + dataPrep(p_data.MFS_y) + "', '" + dataPrep(p_data.MFS_z) + "', '" + dataPrep(p_data.THI_t) + "', '" + dataPrep(p_data.THI_h) + "', '" + dataPrep(p_data.THO_t) + "', '" + dataPrep(p_data.THO_h) + "', '" + dataPrep(p_data.TPO_t) + "', '" + dataPrep(p_data.TPO_p) + "', '" + dataPrep(p_data.WND_s) + "', '" + dataPrep(p_data.WND_o) + "', '" + p_data.LLS.ToString() + "', '" + p_data.RDS.ToString() + "', '" + dataPrep(p_data.IRS) + "');";
                    Console.WriteLine("Flag 4");
                    MySqlCommand command = new MySqlCommand(qery, conn);
                    int name = command.ExecuteNonQuery();
                    Console.WriteLine(name);
                    conn.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message);  }
        }

        private string dataPrep (float given_value)
        {
            double value = given_value;
            value = Math.Round(value, 2);
            string prep = value.ToString();
            return prep.Replace(",", ".");
        }
    }
}
