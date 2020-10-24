using System;
using System.Threading.Tasks;

namespace Downloader
{
    public static class Utils
    {
        public static void Log(string caller, string msg, bool debug = false)
        {
            if (debug && !Program.verbose) return;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{caller}] ");
            Console.ResetColor();
            Console.Write($"{msg}\n");
        }

        public static Task<int> RunProcessAsync(System.Diagnostics.ProcessStartInfo startInfo)
        {
            var tcs = new TaskCompletionSource<int>();
            if (Program.test) tcs.SetResult(0);
            else
            {
                var process = new System.Diagnostics.Process { StartInfo = startInfo, EnableRaisingEvents = true };
                process.Exited += (sender, args) => { tcs.SetResult(process.ExitCode); process.Dispose(); };
                process.Start();
            }
            return tcs.Task;
        }
    }
}
