using System;

namespace Sensors
{
    class Program
    {
        static void Main(string[] args)
        {
            ArProcess np = new ArProcess();

            while (true)
            {
                np.ProcessArData();
                Console.WriteLine("FLag");
            }
        }
    }
}
