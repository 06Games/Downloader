using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Downloader
{
    public class FranceTV
    {
        public static async Task GetVideo(string inputUrl)
        {
            Console.WriteLine("[France TV] Downloading webpage");
            var page = await Network.GetAsync(inputUrl);
            var video_id = System.Text.RegularExpressions.Regex.Match(page, "(?:data-main-video\\s*=|videoId[\"\\']?\\s*[:=])\\s*([\"\\'])(?<id>(?:(?!\\1).)+)\\1").Groups["id"];

            Console.WriteLine("[France TV] Getting informations");
            var progInfos = await Network.GetAsync($"https://sivideo.webservices.francetelevisions.fr/tools/getInfosOeuvre/v2/?idDiffusion={video_id}");
            var infos = JObject.Parse(progInfos);

            Console.WriteLine("[France TV] Downloading streaming playlist");
            var streamUrl = await Network.GetAsync($"https://hdfauthftv-a.akamaihd.net/esi/TA?url={infos.SelectToken("videos[0]").Value<string>("url")}");

            Dictionary<string, string> _args = new Dictionary<string, string>
            {
                { "y", null },
                {"headers", "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.21 Safari/537.36\\r\\nAccept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7\\r\\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\\r\\nAccept-Encoding: gzip, deflate\\r\\nAccept-Language: en-us,en;q=0.5\\r\\n" },
                { "i", streamUrl },
                { "c", "copy" },
                { "f", "flv" }
            };
            var args = _args.Select(a => $"-{a.Key}" + (a.Value != null ? $" \"{a.Value}\"" : ""));
            var cmd = $"./ffmpeg.exe {args} output.flv";
            Console.WriteLine($"[France TV] Launching ffmpeg with the following arguments:\n\t{string.Join("\n\t", args)}");
            await RunProcessAsync(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"{string.Join(" ", args)} output.flv",
                RedirectStandardOutput = true,
            });
        }

        static Task<int> RunProcessAsync(System.Diagnostics.ProcessStartInfo startInfo)
        {
            var tcs = new TaskCompletionSource<int>();
            var process = new System.Diagnostics.Process { StartInfo = startInfo, EnableRaisingEvents = true };
            process.Exited += (sender, args) => { tcs.SetResult(process.ExitCode); process.Dispose(); };
            process.Start();
            return tcs.Task;
        }
    }
}
