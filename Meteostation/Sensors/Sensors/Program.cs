using System;
using System.Threading;

namespace Sensors
{
    class Program
    {
        static void Main(string[] args)
        {
            ArProcess np = new ArProcess();
            Kostil cam = new Kostil();

            Thread camThread = new Thread(cam.CamThr);
            //camThread.Start();

            while (true)
            {
                Console.WriteLine("Ar Start");
                np.ProcessArData();
                Console.WriteLine("ArFLag");
            }
        }

    }

    class Kostil
    {
        CamProcess cp = new CamProcess();

        public void CamThr()
        {
            while (true)
            {
                Console.WriteLine("Cam Start");
                cp.ProcessCamData();
                Console.WriteLine("CamFLag");
            }
        }
    }
}
