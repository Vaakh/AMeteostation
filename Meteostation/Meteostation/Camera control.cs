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

namespace CameraControl
{   
    class SkyCam
    {
        public void StartCanonAPI()
        {
            EDSDK.NET.STAThread.Create(CameraThread);
        }

        public void CameraThread()
        {
            
        }

        static void Main()
        {
            uint port = 0;
            IntPtr ports;
            port = EdsGetCameraList(out ports);
            Console.WriteLine(port);
            Thread.Sleep(1000);
            EDSDK.NET.Camera camera;
        }
    }
}

