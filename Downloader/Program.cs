using System;
using System.Threading.Tasks;

namespace Downloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 1) Console.WriteLine("Error");
            else await FranceTV.GetVideo(args[0]);
        }
    }
}
