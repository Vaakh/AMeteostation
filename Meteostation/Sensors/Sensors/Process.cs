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
            while (!ar_process_shutdown)
            {
                try
                {
                    Process arProc = new Process();
                    Arduino arduino = new Arduino();

                    while (!arduino.FindArPort())
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine("Finding Ar port");
                    }

                    Console.WriteLine("ArduinoPort found");

                    while (!ar_process_shutdown)
                    {
                        ar_data = arduino.ReadArData();
                        ArduinoData p_data = JsonConvert.DeserializeObject<ArduinoData>(ar_data);
                        DateTime thisDay = DateTime.Today;

                        string part_con = "server=localhost;user=Meteostation;database=datatest;password=OpenSourse!123";
                        MySqlConnection conn = new MySqlConnection(part_con);

                        conn.Open();
                        string qery;
                        int currentId;
                        //string tableName = thisDay.Year.ToString() + "/" + thisDay.Month.ToString() + "/" + thisDay.Day.ToString();
                        string tableName = p_data.RTC_date;

                        bool exists = arProc.isTableExists(tableName, conn);
                        if (exists == false)
                        {
                            qery = "CREATE TABLE `datatest`.`" + tableName + "` ( `DataID` INT NOT NULL, `RTC_time` TIME NULL, `MFS_x` FLOAT NULL, `MFS_y` FLOAT NULL, `MFS_z` FLOAT NULL, " +
                                                                                "`THI_t` FLOAT NULL, `THI_h` FLOAT NULL, `THO_t` FLOAT NULL, `THO_h` FLOAT NULL, `TPO_t` FLOAT NULL, `TPO_p` FLOAT NULL, " +
                                                                                "`WND_s` FLOAT NULL, `WND_o` FLOAT NULL, `LLS` INT NULL, `RDS` INT NULL, `IRS` FLOAT NULL, `photo_path` VARCHAR(120) NULL, " +
                                                                                "PRIMARY KEY(`DataID`), UNIQUE INDEX `DataID_UNIQUE` (`DataID` ASC) VISIBLE); INSERT INTO `datatest`.`" + tableName + "` (`DataID`) VALUES('0');";
                            MySqlCommand createTable = new MySqlCommand(qery, conn);
                            createTable.ExecuteNonQuery();
                        }
                        qery = "SELECT MAX(`DataID`) FROM `datatest`.`" + tableName + "`;";
                        MySqlCommand getId = new MySqlCommand(qery, conn);
                        currentId = Convert.ToInt32(getId.ExecuteScalar().ToString()) + 1;
                        qery = "INSERT INTO `datatest`.`" + tableName + "` (`DataID`, `RTC_time`, `MFS_x`, `MFS_y`, `MFS_z`, `THI_t`, `THI_h`, " +
                                                                            "`THO_t`, `THO_h`, `TPO_t`, `TPO_p`, `WND_s`, `WND_o`, `LLS`, `RDS`, `IRS`) " +
                                                                            "VALUES('" + currentId.ToString() + "', '" + p_data.RTC_time + "', '" + 
                                                                            dataPrep(p_data.MFS_x) + "', '" + dataPrep(p_data.MFS_y) + "', '" + dataPrep(p_data.MFS_z) + "', '" + 
                                                                            dataPrep(p_data.THI_t) + "', '" + dataPrep(p_data.THI_h) + "', '" + dataPrep(p_data.THO_t) + "', '" + dataPrep(p_data.THO_h) + "', '" + 
                                                                            dataPrep(p_data.TPO_t) + "', '" + dataPrep(p_data.TPO_p) + "', '" + dataPrep(p_data.WND_s) + "', '" + dataPrep(p_data.WND_o) + "', '" + 
                                                                            p_data.LLS.ToString() + "', '" + p_data.RDS.ToString() + "', '" + dataPrep(p_data.IRS) + "');";
                        MySqlCommand command = new MySqlCommand(qery, conn);
                        int name = command.ExecuteNonQuery();

                        conn.Close();
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }

        }

        private string dataPrep(float given_value)
        {
            double value = given_value;
            value = Math.Round(value, 2);
            string prep = value.ToString();
            return prep.Replace(",", ".");
        }
    }

    class CamProcess
    {
        public bool cam_process_shutdown = false;
        string[] weatherData;
        string[] settings;
        string[] autoSettings;
        string[] currentSettings;
        int minLLSLevel = 1024;

        public void ProcessCamData()
        {
            while (!cam_process_shutdown)
            {
                try
                {
                    Process camProc = new Process();
                    AllSkyCam cam = new AllSkyCam();

                    Console.WriteLine("Test Flag Cam");

                    while (!cam.Initialize())
                    {
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine("Camera has been initialized");

                    while (!cam_process_shutdown)
                    {
                        DateTime thisDay = DateTime.Today;

                        string part_con = "server=localhost;user=Meteostation;database=datatest;password=OpenSourse!123";
                        MySqlConnection conn = new MySqlConnection(part_con);

                        conn.Open();
                        string qery;
                        string tableName = thisDay.Year.ToString() + "/" + thisDay.Month.ToString() + "/" + thisDay.Day.ToString();

                        while (!camProc.isTableExists(tableName, conn))
                        {
                            Thread.Sleep(1000);
                        }

                        qery = "SELECT `DataID`, `RTC_time`, `THO_t`, `THO_h`, `TPO_t`, `LLS`, `RDS`, `IRS` FROM `datatest`.`" + tableName + "` WHERE `DataID` = MAX(`DataID`); ";
                        MySqlCommand getArData = new MySqlCommand(qery, conn);
                        MySqlDataReader dataReader = getArData.ExecuteReader();
                        for (int i = 0; i < 7; i++)
                        {
                            weatherData[i] = dataReader[i].ToString();
                        }
                        dataReader.Close();

                        if (IsAdvisableToTakePhoto(weatherData))
                        {
                            string userSetTabName = "ucamsettings";
                            if (camProc.isTableExists(userSetTabName, conn))
                            {
                                qery = "SELECT `Use User settings`, `ISO`, `Tv`, `Av`  FROM `datatest`.`" + userSetTabName + "` WHERE idUCamSettings = '2'; ";
                                MySqlCommand getSetData = new MySqlCommand(qery, conn);
                                MySqlDataReader reader = getSetData.ExecuteReader();
                                for (int i = 0; i < 7; i++)
                                {
                                    settings[i] = reader[i].ToString();
                                }
                                dataReader.Close();

                                if (settings[0].Contains("Yes"))
                                {
                                    currentSettings[0] = settings[1];
                                    currentSettings[1] = settings[2];
                                    currentSettings[2] = ConvertToBulb(settings[2]);
                                    currentSettings[3] = settings[3];
                                }
                                else { currentSettings = AutoSettings(weatherData); }
                            }
                            else { currentSettings = AutoSettings(weatherData); }

                            cam.TakePhoto(currentSettings);

                            qery = "UPDATE `datatest`.`" + tableName + "` SET `photo_path` = '" + Path.Combine(AllSkyCam.ImageSaveDirectory, AllSkyCam.FileName) + "' WHERE (`DataID` = '" + weatherData[0] + "');";
                            MySqlCommand command = new MySqlCommand(qery, conn);
                            int name = command.ExecuteNonQuery();
                        }

                        conn.Close();
                        Thread.Sleep(6000);
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
        }

        private bool IsAdvisableToTakePhoto(string[] data)
        {
            return true;
        }

        private string ConvertToBulb(string Tv)
        {

            if (Tv.Contains("(1/3)"))
            {
                string[] tvSplit = Tv.Split(" ", 2);
                Tv = tvSplit[1];
            }

            if (Tv.Contains("\""))
            {
                Tv.Replace("\"", ",");
                Tv = Math.Round(Double.Parse(Tv) * 1000).ToString();
            }

            if (Tv.Contains("/"))
            {
                string[] divTv = Tv.Split("/", 2);
                Tv = Math.Round((Double.Parse(divTv[1]) / Double.Parse(divTv[2])) * 1000).ToString();
            }

            return Tv;
        }

        private string[] AutoSettings(string[] weatherData)
        {

            if (int.Parse(weatherData[6]) < minLLSLevel)
            {
                autoSettings[0] = "Auto";
                autoSettings[1] = "Auto";
                autoSettings[2] = "Auto";
                autoSettings[3] = "Auto";
            }
            else
            {
                autoSettings[0] = "ISO 3200";
                autoSettings[1] = "30\"";
                autoSettings[2] = ConvertToBulb(autoSettings[1]);
                autoSettings[3] = "2.8";
            }
            return autoSettings;
        }
    }

}
