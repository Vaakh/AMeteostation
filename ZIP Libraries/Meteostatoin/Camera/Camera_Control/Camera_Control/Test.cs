using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static EDSDKLib.EDSDK;

namespace Camera_Control
{


    class Test
    {
        static EDSDK.NET.SDKHandler CameraHandler;
        static bool WaitForEvent;

        static void Main(string[] args)
        {
            try
            {
                CameraHandler = new EDSDK.NET.SDKHandler();
                CameraHandler.SDKObjectEvent += handler_SDKObjectEvent;
                List<EDSDK.NET.Camera> cameras = CameraHandler.GetCameraList();
                if (cameras.Count > 0)
                {
                    CameraHandler.OpenSession(cameras[0]);
                    Console.WriteLine("Opened session with camera: " + cameras[0].Info.szDeviceDescription);
                }
                else
                {
                    Console.WriteLine("No camera found. Please plug in camera");
                    CameraHandler.CameraAdded += handler_CameraAdded;
                    CallEvent();
                }

                CameraHandler.ImageSaveDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "RemotePhoto");
                CameraHandler.SetSetting(PropID_SaveTo, (uint)EdsSaveTo.Host);

                Console.WriteLine("Taking photo with current settings...");
                CameraHandler.TakePhoto();

                CallEvent();
                Console.WriteLine("Photo taken and saved");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            finally
            {
                CameraHandler.CloseSession();
                CameraHandler.Dispose();
                Console.WriteLine("Good bye! (press any key to close)");
                Console.ReadKey();
            }
        }

        static void CallEvent()
        {
            WaitForEvent = true;
            while (WaitForEvent)
            {
                EdsGetEvent();
                Thread.Sleep(200);
            }
        }

        static uint handler_SDKObjectEvent(uint inEvent, IntPtr inRef, IntPtr inContext)
        {
            if (inEvent == ObjectEvent_DirItemRequestTransfer || inEvent == ObjectEvent_DirItemCreated)
                WaitForEvent = false;
            return EDS_ERR_OK;
        }

        static void handler_CameraAdded()
        {
            List<EDSDK.NET.Camera> cameras = CameraHandler.GetCameraList();
            if (cameras.Count > 0) CameraHandler.OpenSession(cameras[0]);
            Console.WriteLine("Opened session with camera: " + cameras[0].Info.szDeviceDescription);
            WaitForEvent = false;
        }
    }
}
