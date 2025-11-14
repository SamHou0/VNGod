using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using HandyControl.Tools.Converter;
using System.Windows.Media.Animation;
using log4net;

namespace VNGod.Services
{
    internal static class CompressHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CompressHelper));
        public static async Task CompressFolderToZipAsync(string folderPath, string zipFilePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                    }
                    ZipFile.CreateFromDirectory(folderPath, zipFilePath, CompressionLevel.SmallestSize, false,Encoding.UTF8);
                    File.SetLastWriteTime(zipFilePath, Directory.GetLastWriteTime(folderPath));// Keep the access time consistent
                }
                catch (Exception ex)
                {
                    logger.Error($"Error compressing folder: {ex.Message}", ex);
                    throw;
                }
            });
        }
        public static async Task DecompressZipToFolderAsync(string zipFilePath, string extractPath)
        {
            await Task.Run(() =>
            {
                try
                {

                    if (!File.Exists(zipFilePath))
                    {
                        throw new FileNotFoundException("Zip file not found.", zipFilePath);
                    }
                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }
                    // Ensure the extraction directory exists
                    Directory.CreateDirectory(extractPath);
                    ZipFile.ExtractToDirectory(zipFilePath, extractPath,Encoding.UTF8);
                    Directory.SetLastWriteTime(extractPath, Directory.GetLastWriteTime(zipFilePath));// Keep the access time consistent
                    return extractPath;
                }
                catch (Exception ex)
                {
                    logger.Error($"Error decompressing zip file: {ex.Message}", ex);
                    throw;
                }
            });
        }
    }
}
