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

        public static async Task<int> RunProcessAsync(System.Diagnostics.ProcessStartInfo startInfo)
        {
            int result = -1;
            if (Program.test) result = 0;
            else
            {
                var process = new System.Diagnostics.Process { StartInfo = startInfo, EnableRaisingEvents = true };
                process.Exited += (sender, args) => { result = process.ExitCode; process.Dispose(); };

                var start = Program.start - DateTime.Now;
                var refreshRate = TimeSpan.FromSeconds(1);
                while (Program.start - DateTime.Now - refreshRate > TimeSpan.Zero)
                {
                    Console.Write($"\rWaiting for {(Program.start - DateTime.Now).ToString("hh\\:mm\\:ss")}");
                    await Task.Delay(refreshRate);
                }
                if (Program.start - DateTime.Now > TimeSpan.Zero) await Task.Delay(Program.start - DateTime.Now);

                process.Start();
                try { while (!process.HasExited) await Task.Delay(500); } catch { /* The process has been disposed */ }
            }
            return result;
        }
    }
}
