using log4net;
using System.IO;
using VNGod.Data;
using VNGod.Models;
using VNGod.Network;
using VNGod.Services;

namespace VNGod.Utils
{
    static class WebDAVHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebDAVHelper));

        /// <summary>
        /// Check if the WebDAV client is initialized.
        /// </summary>
        public static bool IsInitialized => WebDAVClient.IsInitialized;

        /// <summary>
        /// Initialize WebDAV client with settings from configuration.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> InitializeClient() => await WebDAVClient.InitializeClient();

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
                    var timeComparison = WebDAVClient.CompareFileDate(remoteMetaPath, localMetaPath);
                    Logger.Info($"Comparing metadata for game {game.DirectoryName}: Time comparison result = {timeComparison}");
                    if (timeComparison == -1 || timeComparison == 404)
                    {
                        if (await WebDAVClient.UploadFileAsync(localMetaPath, remoteMetaPath))
                        {
                            Logger.Info($"Uploaded metadata for game {game.DirectoryName}.");
                        }
                        else
                            throw new Exception("Upload metadata failed.");
                    }
                    else if (timeComparison == 1)
                    {
                        if (await WebDAVClient.DownloadFileAsync(remoteMetaPath, localMetaPath))
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
                    await CompressHelper.CompressFolderToZipAsync(localSavePath, tempZipPath);
                    // Compare file dates to determine which is newer
                    timeComparison = WebDAVClient.CompareFileDate(remoteSavePath, tempZipPath);
                }
                if (timeComparison == -1 || timeComparison == 404)
                {
                    Logger.Info("Local save is newer or remote not found. Uploading...");
                    // Upload the zip file to WebDAV server
                    var uploadSuccess = await WebDAVClient.UploadFileAsync(tempZipPath, remoteSavePath);
                    if (!uploadSuccess)
                    {
                        Logger.Error("Upload failed.");
                        return false;
                    }
                }
                else
                {
                    Logger.Info("Remote save is newer or same. Downloading...");
                    var downloadSuccess = await WebDAVClient.DownloadFileAsync(remoteSavePath, tempZipPath);
                    if (!downloadSuccess)
                    {
                        Logger.Error("Download failed.");
                        return false;
                    }
                    // Decompress the zip file back to the local save folder
                    await CompressHelper.DecompressZipToFolderAsync(tempZipPath, localSavePath);
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
        public static async Task<bool> UploadGameAsync(Repo repo, Game game, IProgress<StagedProgressInfo> progress)
        {
            try
            {
                string tmpPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", game.DirectoryName);
                if (Directory.Exists(tmpPath))
                    Directory.Delete(tmpPath, true);
                if (!await WebDAVClient.DeleteRemoteAsync($"{game.DirectoryName}/game"))
                    Logger.Warn("Failed to delete remote game folder before upload. It may not exist.");
                Directory.CreateDirectory(tmpPath);
                await CompressHelper.CompressSplitZipFileAsync(Path.Combine(tmpPath, "game"), Path.Combine(repo.LocalPath, game.DirectoryName), progress);
                progress.Report(new StagedProgressInfo { StagePercentage = 0, StageName = "Preparing upload..." });
                string[] files = Directory.GetFiles(tmpPath);

                for (int i = 0; i < files.Length; i++)
                {
                    if (await WebDAVClient.UploadFileAsync(files[i], $"{game.DirectoryName}/game/{Path.GetFileName(files[i])}") == false)
                        throw new Exception($"Error uploading " + files[i]);
                    progress.Report(new StagedProgressInfo { StagePercentage = (double)(i + 1) / files.Length * 100, StageName = "Uploading files..." });
                }
                progress.Report(new StagedProgressInfo { StagePercentage = 100, StageName = "Upload complete" });
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error when compressing and uploading game: {ex.Message}", ex);
                return false;
            }
        }
    }
}
