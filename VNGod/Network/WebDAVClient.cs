using log4net;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Navigation;
using VNGod.Properties;
using WebDav;

namespace VNGod.Network
{
    /// <summary>
    /// Static class for managing WebDAV client and basic operations.
    /// </summary>
    static class WebDAVClient
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebDAVClient));
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
            if (string.IsNullOrEmpty(Settings.Default.WebDAVUrl)||
                Settings.Default.WebDAVUrl=="/" // Auto addition, avoid this
                || string.IsNullOrEmpty(Settings.Default.WebDAVUsername) || string.IsNullOrEmpty(Settings.Default.WebDAVPassword))
            {
                // Clear old client
                client = null;
                return false;
            }
            try
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

            }
            catch (Exception ex)
            {
                Logger.Error($"Exception during WebDAV client initialization: {ex.Message}", ex);
                client = null;
            }
            return false;

        }
        private async static Task<string?> GetBaseUriAsync(string requestUri)
        {
            var response = await client!.Propfind(requestUri, new() { Headers = new Dictionary<string, string> { { "Depth", "0" } } });
            if (response.IsSuccessful)
            {
                return response.Resources.First().Uri!.ToString();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Test WebDAV connection.
        /// </summary>
        /// <param name="testClient">WebDAV client to test</param>
        /// <returns></returns>
        public static async Task<bool> TestConnectionAsync(WebDavClient testClient)
        {
            try
            {
                var response = await testClient.Propfind("");
                if (response.IsSuccessful)
                {
                    Logger.Debug("WebDAV connection successful.");
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
        /// <summary>
        /// List remote directory contents.
        /// </summary>
        /// <param name="requestUri">The request uri to search sub dir</param>
        /// <param name="remoteFileList">The list to add result in</param>
        /// <returns>If the operation is successful</returns>
        public static async Task<bool> ListRemoteAsync(string requestUri, List<string> remoteFileList)
        {
            try
            {
                string? baseUri = await GetBaseUriAsync(requestUri);
                if (baseUri == null) return false;
                var response = await client!.Propfind(requestUri);
                if (response.IsSuccessful)
                {
                    foreach (var resource in response.Resources)
                    {
                        if (resource.Uri!.ToString() == baseUri) continue; // Skip root dir itself
                        remoteFileList.Add(resource.Uri!.ToString());
                    }
                    Logger.Debug($"Successfully listed files in remote");
                    return true;
                }
                else
                {
                    throw new Exception($"Failed to list files in remote. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception during listing files: {ex.Message}", ex);
                return false;
            }
        }
        /// <summary>
        /// Delete a file or directory from the WebDAV server.
        /// </summary>
        /// <param name="remoteFilePath">Remote path to delete</param>
        /// <returns></returns>
        public static async Task<bool> DeleteRemoteAsync(string remoteFilePath)
        {
            try
            {
                await client!.Delete(remoteFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception during file deletion: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Upload a file to the WebDAV server.
        /// </summary>
        /// <param name="localFilePath">Local file path</param>
        /// <param name="remoteFilePath">Remote file path</param>
        /// <returns></returns>
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
                    Logger.Debug($"Successfully uploaded {localFilePath} to {remoteFilePath}");
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

        /// <summary>
        /// Download a file from the WebDAV server.
        /// </summary>
        /// <param name="remoteFilePath">Remote file path</param>
        /// <param name="localFilePath">Local file path</param>
        /// <returns></returns>
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
                    Logger.Debug($"Successfully downloaded {remoteFilePath} to {localFilePath}");
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
                Logger.Error($"Exception during file download {remoteFilePath} to {localFilePath}: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Compare the last modified date of the remote file and the local file.
        /// </summary>
        /// <param name="remoteFilePath">Remote file path</param>
        /// <param name="localFilePath">Local file path</param>
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
    }
}
