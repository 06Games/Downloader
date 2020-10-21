using System;
using System.Linq;
using System.Threading.Tasks;

namespace Downloader
{
    public class Program
    {
        public static bool verbose;
        static async Task Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("-h"))
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("   Downloader by EvanG   ");
                Console.WriteLine("-------------------------");
                Console.WriteLine("");
                Console.WriteLine("Usage: Downloader [args] [url]");
                Console.WriteLine("\n");
                Console.WriteLine("-h\tDisplays this message");
                Console.WriteLine("-v\tDisplays more detailed logs");
                Console.WriteLine("");
                return;
            }

            verbose = args.Contains("-v");

            var url = args.LastOrDefault();
            if (url.StartsWith("-"))
            {
                Console.WriteLine("The obligatory 'url' parameter is missing");
                Console.WriteLine("Learn more with -h");
                return;
            }
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                Console.WriteLine("Invalid url");
                return;
            }
            await FranceTV.GetVideo(uri.AbsoluteUri);
        }
    }
}
