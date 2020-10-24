using System;
using System.Linq;
using System.Threading.Tasks;

namespace Downloader
{
    public class Program
    {
        public static string output;
        public static bool verbose;
        public static bool test;

        readonly static string[] testUrls = new[] {
            //France TV
            "https://www.france.tv/france-2/direct.html", //France 2 live
            "https://www.france.tv/france-3/lego-ninjago/saison-1/235125-la-legende-des-serpents.html" //Replay
        };

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
                Console.WriteLine("-o\tThe output path (default is current dir)");
                Console.WriteLine("-test\tSend requests to test that the websites are correctly implemented");
                Console.WriteLine("");
                return;
            }

            verbose = args.Contains("-v");
            test = args.Contains("-test");
            output = args.Contains("-o") ? args[Array.IndexOf(args, "-o") + 1].TrimEnd('/', '\\') + System.IO.Path.DirectorySeparatorChar : "";

            var url = args.LastOrDefault();
            if (test)
            {
                foreach (var URL in testUrls) await FranceTV.GetVideo(URL);
                return;
            }
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
