using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EOSDigital.API;
using EOSDigital.SDK;

namespace ConsoleExample
{
    class Program
    {
        static CanonAPI APIHandler;
        static Camera MainCamera;
        public static string ImageSaveDirectory;
        public static string FileName;
        static bool Error = false;
        static ManualResetEvent WaitEvent = new ManualResetEvent(false);
        private string errorMessage;
        string[] autoSettings = { "Auto", "Auto", "Auto", "Auto" };

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

        public bool TakePhoto(string[] camSettings)  //where cS[0] is ISO(sensitivity), cS[1] is Tv(exposition), cS[2] is Tv in bulb mode and cS[3] is Av(apperture)
        {
            try
            {
                APIHandler = new CanonAPI();
                if (OpenSession())
                {
                    if (!Error)
                    {
                        ImageSaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "RemotePhoto");
                        MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Host);
                        MainCamera.SetSetting(PropertyID.ISO, ISOValues.GetValue(camSettings[0]).IntValue);
                        MainCamera.SetSetting(PropertyID.Av, AvValues.GetValue(camSettings[3]).IntValue);
                        MainCamera.SetCapacity(4096, int.MaxValue);
                        Console.WriteLine($"Set image output path to: {ImageSaveDirectory}");

                        Console.WriteLine("Taking photo with special settings...");
                        CameraValue tv = TvValues.GetValue(MainCamera.GetInt32Setting(PropertyID.Tv));
                        if (tv == TvValues.Bulb) MainCamera.TakePhotoBulb(int.Parse(camSettings[2]));
                        else
                        {
                            MainCamera.SetSetting(PropertyID.Tv, TvValues.GetValue(camSettings[1]).IntValue);
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
                FileName = Info.FileName;
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

        static void Main()
        {
            Program p = new Program();
            p.TakePhoto(p.autoSettings);
            Console.WriteLine("The end");
            Console.ReadKey();
        }
    }
}
