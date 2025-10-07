using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using VNGod.Data;
using VNGod.Properties;
using WebDav;
using log4net;
using log4net.Repository.Hierarchy;

namespace VNGod.Services
{
    static class WebDavService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebDavService));
        private static WebDavClient? client;
        /// <summary>
        /// Check if the WebDAV client is initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return client != null;
            }
        }
        /// <summary>
        /// Initialize WebDAV client with settings from configuration.
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> InitializeClient()
        {
            var clientParams = new WebDavClientParams
            {
                BaseAddress = new Uri(Settings.Default.WebDAVUrl),
                Credentials = new NetworkCredential(Settings.Default.WebDAVUsername, Settings.Default.WebDAVPassword)
            };
            var testClient = new WebDavClient(clientParams);
            for (int i = 1; i <= 4; i++)
            {
                if (await TestConnectionAsync(testClient))
                {
                    client = testClient;
                    return true;
                }
            }
            return false;
        }
        public static async Task<bool> TestConnectionAsync(WebDavClient client)
        {
            try
            {
                var response = await client.Propfind("");
                if (response.IsSuccessful)
                {
                    Logger.Info("WebDAV connection successful.");
                    return true;
                }
                else
                {
                    Logger.Error($"WebDAV connection failed. Status code: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception during WebDAV connection test: {ex.Message}", ex);
                return false;
            }
        }
        public static async Task<bool> UploadFileAsync(string localFilePath, string remoteFilePath)
        {
            try
            {
                if (!File.Exists(localFilePath))
                {
                    Logger.Error($"Local file does not exist: {localFilePath}");
                    return false;
                }
                using var fileStream = File.OpenRead(localFilePath);
                var response = await client!.PutFile(remoteFilePath, fileStream);
                if (response.IsSuccessful)
                {
                    Logger.Info($"Successfully uploaded {localFilePath} to {remoteFilePath}");
                    return true;
                }
                else
                {
                    Logger.Error($"Failed to upload {localFilePath} to {remoteFilePath}. Status code: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception during file upload: {ex.Message}", ex);
                return false;
            }
        }
        public static async Task<bool> DownloadFileAsync(string remoteFilePath, string localFilePath)
        {
            try
            {
                var response = await client!.GetRawFile(remoteFilePath);
                DateTime? time = client.Propfind(remoteFilePath).Result.Resources.First().LastModifiedDate;
                if (response.IsSuccessful)
                {
                    using (var fileStream = File.Create(localFilePath))
                    {
                        await response.Stream.CopyToAsync(fileStream);
                    }
                    File.SetLastAccessTime(localFilePath, time ?? throw new NullReferenceException("Null Remote Time."));//Keep the access time consistent
                    Logger.Info($"Successfully downloaded {remoteFilePath} to {localFilePath}");
                    return true;
                }
                else
                {
                    Logger.Error($"Failed to download {remoteFilePath}. Status code: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception during file download: {ex.Message}", ex);
                return false;
            }
        }
        /// <summary>
        /// Compare the last modified date of the remote file and the local file.
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="localFilePath"></param>
        /// <returns>1 if remote newer, -1 if local newer, 0 if the same, 404 if not found.</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static int CompareFileDate(string remoteFilePath, string localFilePath)
        {
            var resources = client!.Propfind(remoteFilePath).Result.Resources;
            if (resources.Count > 0)
            {
                DateTime remoteTime = resources.First().LastModifiedDate ?? throw new NullReferenceException("Null Remote Time.");
                DateTime localTime = File.GetLastWriteTime(localFilePath);
                if (remoteTime > localTime)
                {
                    return 1;//Remote is newer
                }
                else if (remoteTime < localTime)
                {
                    return -1;//Local is newer
                }
                else
                {
                    return 0;//Same
                }
            }
            else return 404;//Not found
        }
        /// <summary>
        /// Synchronize metadata files (.vngod) for all games in the repo.
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        public static async Task<bool> SyncMetadataAsync(Repo repo)
        {
            foreach (Game game in repo)
            {
                try
                {
                    var localMetaPath = Path.Combine(repo.LocalPath, game.DirectoryName, ".vngod");
                    var remoteMetaPath = $"{game.DirectoryName}/.vngod";
                    var timeComparison = CompareFileDate(remoteMetaPath, localMetaPath);
                    Logger.Info($"Comparing metadata for game {game.DirectoryName}: Time comparison result = {timeComparison}");
                    if (timeComparison == -1 || timeComparison == 404)
                    {
                        if (await UploadFileAsync(localMetaPath, remoteMetaPath))
                        {
                            Logger.Info($"Uploaded metadata for game {game.DirectoryName}.");
                        }
                        else
                            throw new Exception("Upload metadata failed.");
                    }
                    else if (timeComparison == 1)
                    {
                        if (await DownloadFileAsync(remoteMetaPath, localMetaPath))
                        {
                            Logger.Info($"Downloaded metadata for game {game.DirectoryName}.");
                        }
                        else
                            throw new Exception("Download metadata failed.");
                    }
                    else
                    {
                        Logger.Info($"Metadata for game {game.DirectoryName} is up to date.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Exception during metadata sync for game {game.DirectoryName}: {ex.Message}", ex);
                    return false;
                }
            }
            return true;
        }
        public static async Task<bool> SyncGameAsync(Game game)
        {
            if (string.IsNullOrEmpty(game.SavePath) || string.IsNullOrEmpty(game.DirectoryName))
            {
                Logger.Error("Game save path or directory name is not set.");
                return false;
            }
            var localSavePath = game.SavePath;
            var remoteSavePath = $"{game.DirectoryName}/save.zip";
            var tempZipPath = Path.Combine(Path.GetTempPath(), $"{game.DirectoryName}_save.zip");
            try
            {

                int timeComparison = 1;
                // Ensure remote file not overwrite by lost local folder
                if (Directory.Exists(localSavePath))
                {
                    // Compress local save folder to a temporary zip file
                    await CompressService.CompressFolderToZipAsync(localSavePath, tempZipPath);
                    // Compare file dates to determine which is newer
                    timeComparison = CompareFileDate(remoteSavePath, tempZipPath);
                }
                if (timeComparison == -1 || timeComparison == 404)
                {
                    Logger.Info("Local save is newer or remote not found. Uploading...");
                    // Upload the zip file to WebDAV server
                    var uploadSuccess = await UploadFileAsync(tempZipPath, remoteSavePath);
                    if (!uploadSuccess)
                    {
                        Logger.Error("Upload failed.");
                        return false;
                    }
                }
                else
                {
                    Logger.Info("Remote save is newer or same. Downloading...");
                    var downloadSuccess = await DownloadFileAsync(remoteSavePath, tempZipPath);
                    if (!downloadSuccess)
                    {
                        Logger.Error("Download failed.");
                        return false;
                    }
                    // Decompress the zip file back to the local save folder
                    await CompressService.DecompressZipToFolderAsync(tempZipPath, localSavePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception during game sync: {ex.Message}", ex);
                return false;
            }
            finally
            {
                // Clean up temporary zip file
                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }
            }
        }
    }
}
