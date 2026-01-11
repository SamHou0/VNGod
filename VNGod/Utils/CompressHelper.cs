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
using System.Diagnostics;
using VNGod.Properties;
using VNGod.Models;

namespace VNGod.Services
{
    internal static class CompressHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CompressHelper));
        private static List<Process> compressionProcesses = new();
        // Compression and decompression methods
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
                    ZipFile.CreateFromDirectory(folderPath, zipFilePath, CompressionLevel.SmallestSize, false, Encoding.UTF8);
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
                    ZipFile.ExtractToDirectory(zipFilePath, extractPath, Encoding.UTF8);
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
        // Split a large zip file into smaller parts
        public static async Task CompressSplitZipFileAsync(string zipFilePath, string folderPath,  IProgress<StagedProgressInfo> progress,int partSize=100)
        {
            string sevenZipPath = Settings.Default.SevenZipPath;
            ProcessStartInfo processStartInfo = new()
            {
                FileName = sevenZipPath,
                // Compress, and ignore .vngod files, split into parts
                Arguments = $"a -bsp1 -bb1 -x!*\\.vngod -v{partSize}m \"{zipFilePath}\" \"{folderPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using Process process = Process.Start(processStartInfo) ?? throw new Exception("Error starting 7-zip. Check your path.");
            compressionProcesses.Add(process);
            process.OutputDataReceived += (sender, e) =>
            {
                //Debug.WriteLine(e.Data);
                if (!string.IsNullOrEmpty(e.Data))
                {
                    int index = e.Data.IndexOf('%');
                    // Parse output for progress updates
                    if (index>=0)
                    {
                        string percentStr = e.Data.Substring(index-2,2);
                        if (double.TryParse(percentStr, out double percent))
                        {
                            progress.Report(new StagedProgressInfo { StagePercentage = percent, StageName = "Compressing files..." });
                        }
                    }
                }
            };

            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
            compressionProcesses.Remove(process);
        }
        // Extract split zip files
        public static async Task DecompressSplitZipsAsync(string zipFilePath, string extractPath, IProgress<StagedProgressInfo> progress)
        {
            string sevenZipPath = Settings.Default.SevenZipPath;
            ProcessStartInfo processStartInfo = new()
            {
                FileName = sevenZipPath,
                Arguments = $"x -bsp1 -bb1 \"{zipFilePath}\" -o\"{extractPath}\" -y",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using Process process = Process.Start(processStartInfo) ?? throw new Exception("Error starting 7-zip. Check your path.");
            compressionProcesses.Add(process);
            process.OutputDataReceived += (sender, e) =>
            {
                //Debug.WriteLine(e.Data);
                if (!string.IsNullOrEmpty(e.Data))
                {
                    int index = e.Data.IndexOf('%');
                    // Parse output for progress updates
                    if (index >= 0)
                    {
                        string percentStr = e.Data.Substring(index - 2, 2);
                        if (double.TryParse(percentStr, out double percent))
                        {
                            progress.Report(new StagedProgressInfo { StagePercentage = percent, StageName = "Extracting files..." });
                        }
                    }
                }
            };
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
            compressionProcesses.Remove(process);
        }
        public static void CancelAllCompressionProcesses()
        {
            foreach (var process in compressionProcesses)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error($"Error cancelling compression process: {ex.Message}", ex);
                }
            }
            compressionProcesses.Clear();
        }
    }
}
