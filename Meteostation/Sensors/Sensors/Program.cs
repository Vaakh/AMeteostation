using System;

namespace Sensors
{
    class Program
    {
        static void Main(string[] args)
        {
            ArProcess np = new ArProcess();
            CamProcess cp = new CamProcess();

            while (true)
            {
                //np.ProcessArData();
                cp.ProcessCamData();
                Console.WriteLine("FLag");
            }
        }
    }
}
