using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNGod.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace VNGod.Utils
{
    internal static class IconHelper
    {
        public static async void GetIcons(Repo repo)
        {
            foreach (var game in repo)
            {
                if (!string.IsNullOrEmpty(game.ExecutableName))
                {
                    await Task.Run(() =>
                    {
                        //Get the icon from the executable
                        Icon icon = Icon.ExtractAssociatedIcon(Path.Combine(repo.LocalPath, game.DirectoryName, game.ExecutableName)) ?? throw new NullReferenceException("Null icon");
                        using MemoryStream ms = new();
                        //Convert to imageSource
                        icon.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        BitmapImage bitmap = new();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        game.Icon = bitmap;
                    });
                }
            }
        }
    }
}
