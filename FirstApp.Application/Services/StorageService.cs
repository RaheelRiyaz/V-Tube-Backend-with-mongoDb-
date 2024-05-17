using FirstApp.Application.Abstractions.IServices;
using MediaInfo.DotNetWrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.AspNetCore.Hosting;

namespace FirstApp.Application.Services
{
    public class StorageService(string webrootPath) : IStorageService
    {
        public int DeleteLocalFile(string filePath)
        {
            File.Delete(filePath);

            return 0;
        }

        public async Task<string> SaveLocalFile(IFormFile file)
        {
            // Construct the path to save the file in the wwwroot/Files directory
            string filePath = Path.Combine(webrootPath, "Files", file.FileName);

            // Save the file
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return filePath;
        }

        private string GetPhysicalPath()
        {
            string path = webrootPath + "/Files";

            if (!Path.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }


        public string GetVideoDuration(string filePath)
        {
            var ffProbe = new NReco.VideoInfo.FFProbe();
            var videoInfo = ffProbe.GetMediaInfo(filePath);
            var duration = videoInfo.Duration.TotalSeconds.ToString();
            return FormatDuration(Convert.ToDouble(duration));
        }


        private string FormatDuration(double totalSeconds)
        {
            TimeSpan duration = TimeSpan.FromSeconds(totalSeconds);
            return duration.ToString(@"hh\:mm\:ss");
        }

    }
}
