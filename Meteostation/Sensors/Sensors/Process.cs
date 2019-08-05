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
        string parsed_ar_data;
        string part_of_path = System.IO.Directory.GetCurrentDirectory();
        string path;

        public void ProcessArData()
        {
            try
            {
                Process arProc = new Process();
                Arduino arduino = new Arduino();

                while (!arduino.FindArPort())
                {
                    Thread.Sleep(10000);
                }

                while (!ar_process_shutdown)
                {
                    ar_data = arduino.ReadArData();
                    ArduinoData p_data = JsonConvert.DeserializeObject<ArduinoData>(ar_data);
                    DateTime thisDay = DateTime.Today;
                    string part_con = "server=localhost;user=Meteostation;database=datatest;password=OpenSourse!123"; 
                    MySqlConnection conn = new MySqlConnection(part_con);
                    conn.Open();
                    string qery;

                    string tableName = thisDay.Year.ToString() + "/" + thisDay.Month.ToString() + "/" + thisDay.Day.ToString();

                    bool exists = arProc.isTableExists(tableName, conn);

                    if (exists == false)
                    {
                        qery = "CREATE TABLE `datatest`.`" + tableName + "` ( `RTC_time` TIME NOT NULL, `MFS_x` INT NULL, `MFS_y` INT NULL, `MFS_z` INT NULL, `THI_t` FLOAT NULL, `THI_h` FLOAT NULL, `THO_t` FLOAT NULL, `THO_h` FLOAT NULL, `TPO_t` FLOAT NULL, `TPO_p` FLOAT NULL, `WND_s` INT NULL, `WND_o` INT NULL, `LLS` INT NULL, `RDS` INT NULL, `IRS` INT NULL, `photo_path` VARCHAR(120) NOT NULL, PRIMARY KEY(`RTC_time`), UNIQUE INDEX `RTC_time_UNIQUE` (`RTC_time` ASC) VISIBLE);";
                        MySqlCommand createTable = new MySqlCommand(qery, conn);
                        createTable.ExecuteNonQuery();
                    }

                    qery = "INSERT INTO `datatest`.`" + tableName + "` (`RTC_time`, `MFS_x`, `MFS_y`, `MFS_z`, `THI_t`, `THI_h`, `THO_t`, `THO_h`, `TPO_t`, `TPO_p`, `WND_s`, `WND_o`, `LLS`, `RDS`, `IRS`) VALUES('" + p_data.RTC_time + "', '" + p_data.MFS_x.ToString() + "', '" + p_data.MFS_y.ToString() + "', '" + p_data.MFS_z.ToString() + "', '" + p_data.THI_t.ToString() + "', '" + p_data.THI_h.ToString() + "', '" + p_data.THO_t.ToString() + "', '" + p_data.THO_h.ToString() + "', '" + p_data.TPO_t.ToString() + "', '" + p_data.TPO_p.ToString() + "', '" + p_data.WND_s.ToString() + "', '" + p_data.WND_o.ToString() + "', '" + p_data.LLS + "', '" + p_data.RDS.ToString() + "', '" + p_data.IRS.ToString() + "');";
                    MySqlCommand command = new MySqlCommand(qery, conn);
                    int name = command.ExecuteNonQuery();

                    conn.Close();
                }
            }
            catch { }
        }

        
    }
}
