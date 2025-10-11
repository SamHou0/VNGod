using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VNGod.Network
{
    internal static class RandomImage
    {
        private static readonly string baseUrl = "https://www.loliapi.com/acg/pc/";
        private static ILog logger=LogManager.GetLogger(typeof(RandomImage));
        public async static Task<BitmapImage?> GetImageAsync()
        {
            try
            {
                using HttpClient client = new();
                client.Timeout = TimeSpan.FromSeconds(10);
                var response = await client.GetByteArrayAsync(baseUrl);
                using (var stream = new MemoryStream(response))
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze(); // To make it cross-thread accessible
                    return bitmap;
                }
            }
            catch (Exception ex) {
                logger.Error("Error fetching random image: " + ex.Message,ex);
                return null;
            }

        }
    }
}
