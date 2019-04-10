using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EOSDigital.API;
using EOSDigital.SDK;

namespace Meteostation_2._0
{
    class Cameras
    {
        static CanonAPI APIHandler;
        static Camera MainCamera;
        static string ImageSaveDirectory;
        static bool Error = false;
        static ManualResetEvent WaitEvent = new ManualResetEvent(false);
        string[] camerasList;

        private bool Initialize()
        {
            List<Camera> cameras = APIHandler.GetCameraList();
            if (cameras.Count > 0)
            {
                for(int i=0; i<cameras.Count; i++)
                {
                    camerasList[i] = cameras[i].DeviceName;
                }
                return true;
            }
            else return false;
        }



    }
}
