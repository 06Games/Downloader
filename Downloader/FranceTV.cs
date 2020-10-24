using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Downloader
{
    public class FranceTV
    {
        readonly static string headers = "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.21 Safari/537.36\\r\\nAccept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7\\r\\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\\r\\nAccept-Encoding: gzip, deflate\\r\\nAccept-Language: en-us,en;q=0.5\\r\\n";
        readonly static string format = "mkv";

        public static async Task GetVideo(string inputUrl)
        {
            Utils.Log("France TV", "Downloading webpage");
            var page = await Network.GetAsync(inputUrl);
            var video_id = System.Text.RegularExpressions.Regex.Match(page, "(?:data-main-video\\s*=|videoId[\"\\']?\\s*[:=])\\s*([\"\\'])(?<id>(?:(?!\\1).)+)\\1").Groups["id"];
            Utils.Log("France TV", $"Video ID: {video_id}", true);

            Utils.Log("France TV", "Getting informations");
            var infos = JObject.Parse(await Network.GetAsync($"https://sivideo.webservices.francetelevisions.fr/tools/getInfosOeuvre/v2/?idDiffusion={video_id}"));
            Utils.Log("France TV", $"Informations:\n\t{infos.ToString()}", true);

            Utils.Log("France TV", "Downloading streaming playlist");
            var videoFormat = infos.SelectToken("videos").FirstOrDefault(v => Path.GetExtension(v.Value<string>("url")) == ".m3u8");
            Utils.Log("France TV", $"Selected format: {videoFormat}", true);
            var streamUrl = await Network.GetAsync($"https://hdfauthftv-a.akamaihd.net/esi/TA?url={videoFormat.Value<string>("url")}");
            Utils.Log("France TV", $"HLS Url: {streamUrl}", true);

            var subtitles = infos.SelectToken("subtitles").FirstOrDefault(s => s.Value<string>("format") == "vtt");

            var args = $"-y -hide_banner -headers \"{headers}\" -i \"{streamUrl}\"";
            if (subtitles != null) args += $" -i \"{subtitles.Value<string>("url")}\" -c copy -c:s srt";
            else args += $" -c copy";
            if (Program.duration != TimeSpan.Zero) args += $" -t {Program.duration}";
            args += $" \"{Program.output}{infos.Value<string>("titre")} - {infos.Value<string>("sous_titre")}.{format}\"";
            if (Program.verbose) Utils.Log("France TV", $"Launching ffmpeg with the following arguments:\n\t{args}");
            else Utils.Log("France TV", "Launching ffmpeg and starting download");
            await Utils.RunProcessAsync(new System.Diagnostics.ProcessStartInfo { FileName = "ffmpeg.exe", Arguments = args, RedirectStandardOutput = true });
        }
    }
}
