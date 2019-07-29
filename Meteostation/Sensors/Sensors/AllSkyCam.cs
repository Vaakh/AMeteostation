using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EOSDigital.API;
using EOSDigital.SDK;


namespace Sensors
{
    class AllSkyCam
    {
        static CanonAPI APIHandler;
        static Camera MainCamera;
        static string ImageSaveDirectory;
        static bool Error = false;
        static ManualResetEvent WaitEvent = new ManualResetEvent(false);
        string[] autoSettings; //where aS[0] is ISO(sensitivity), aS[1] is Tv(exposition), aS[2] is Tv in bulb mode and aS[3] is Av(apperture)
        string[] currentSettings; //where cS[0] is ISO(sensitivity), cS[1] is Tv(exposition), cS[2] is Tv in bulb mode and cS[3] is Av(apperture)
        private string errorMessage;

        public bool Initialize()
        {
            try
            {
                APIHandler = new CanonAPI();
                List<Camera> cameras = APIHandler.GetCameraList();
                if (!OpenSession())
                {
                    Console.WriteLine("No camera found. Please plug in camera");
                    APIHandler.CameraAdded += APIHandler_CameraAdded;
                    WaitEvent.WaitOne();
                    WaitEvent.Reset();
                    return false;
                }
                else { MainCamera.CloseSession(); Console.WriteLine("Session closed"); return true; }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); return false; }
            finally
            {
                MainCamera?.Dispose();
                APIHandler.Dispose();
            }

        }

        public bool TakePhoto()
        {
            try
            {
                APIHandler = new CanonAPI();
                if (OpenSession())
                {
                    if (!Error)
                    {
                        UpdateCurrentSettings();
                        ImageSaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "RemotePhoto");
                        Console.WriteLine("Flag 1");
                        MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Host);
                        Console.WriteLine("Flag 2");
                        MainCamera.SetSetting(PropertyID.ISO, ISOValues.GetValue(currentSettings[0]).IntValue);
                        Console.WriteLine("Flag 3");
                        MainCamera.SetSetting(PropertyID.Av, AvValues.GetValue(currentSettings[3]).IntValue);
                        Console.WriteLine("Flag 4");
                        MainCamera.SetCapacity(4096, int.MaxValue);
                        Console.WriteLine($"Set image output path to: {ImageSaveDirectory}");

                        Console.WriteLine("Taking photo with special settings...");
                        CameraValue tv = TvValues.GetValue(MainCamera.GetInt32Setting(PropertyID.Tv));
                        if (tv == TvValues.Bulb) MainCamera.TakePhotoBulb(int.Parse(currentSettings[2]));
                        else
                        {
                            MainCamera.SetSetting(PropertyID.Tv, TvValues.GetValue(currentSettings[1]).IntValue);
                            MainCamera.TakePhoto();
                        }
                        WaitEvent.WaitOne();

                        if (!Error) { Console.WriteLine("Photo taken and saved"); return true; }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); return false; }
            finally
            {
                MainCamera?.Dispose();
                APIHandler.Dispose();
            }
        }

        private void UpdateCurrentSettings()
        {
            UpdateAutoSettings();
            for (int i = 0; i <= 4; i++)
            {
                currentSettings[i] = autoSettings[i];
            }
        }

        private void UpdateAutoSettings()
        {
            autoSettings[0] = "ISO 100";
            autoSettings[1] = "1\"";
            autoSettings[2] = "250";
            autoSettings[3] = "5.6";
        }

        private static void APIHandler_CameraAdded(CanonAPI sender)
        {
            try
            {
                Console.WriteLine("Camera added event received");
                if (!OpenSession()) { Console.WriteLine("Sorry, something went wrong. This camera is unavailable"); Error = true; }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); Error = true; }
            finally { WaitEvent.Set(); }
        }

        private static void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
            try
            {
                Console.WriteLine("Starting image download...");
                sender.DownloadFile(Info, ImageSaveDirectory);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); Error = true; }
            finally { WaitEvent.Set(); }
        }

        private static bool OpenSession()
        {
            List<Camera> cameras = APIHandler.GetCameraList();
            if (cameras.Count > 0)
            {
                MainCamera = cameras[0];
                MainCamera.DownloadReady += MainCamera_DownloadReady;
                MainCamera.OpenSession();
                Console.WriteLine($"Opened session with camera: {MainCamera.DeviceName}");
                return true;
            }
            else return false;
        }
    }
}
