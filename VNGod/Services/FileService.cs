using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using VNGod.Data;
using log4net.Core;
using log4net;

namespace VNGod.Services
{
    static class FileService
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(FileService));
        /// <summary>
        /// Initializes a repo at the given path by reading its metadata and scanning for games.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Repo InitializeRepo(string path)
        {
            Repo repo = new() { LocalPath = path };
            GetRepoInformation(repo);
            ScanGames(repo);
            return repo;
        }
        /// <summary>
        /// Reads the game directories in the repo and loads their metadata if available.
        /// </summary>
        /// <param name="repo"></param>
        /// <exception cref="Exception"></exception>
        public static void ScanGames(Repo repo)
        {
            repo.Clear();
            var gameDirs = Directory.GetDirectories(repo.LocalPath);
            foreach (var dir in gameDirs)
            {
                try
                {
                    // Check if .vngod file exists
                    if (File.Exists(Path.Combine(dir, ".vngod")))
                    {
                        using StreamReader reader = new(Path.Combine(dir, ".vngod"));
                        XmlSerializer serializer = new(typeof(Game));
                        Game game = (Game?)serializer.Deserialize(reader) ?? throw new Exception("Null game");
                        repo.Add(game);
                    }
                    else
                    {
                        // Initialize game without metadata
                        var game = new Game()
                        {
                            DirectoryName = new DirectoryInfo(dir).Name
                        };
                        repo.Add(game);
                    }
                }
                catch (Exception ex)
                {
                    Exception scanEx = new($"Failed to read .vngod file in {dir}. Not a valid game directory.", ex);
                    logger.Error(scanEx.Message, scanEx);
                    throw scanEx;
                }
            }
        }
        /// <summary>
        /// Save game and repo metadata to disk.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="overwrite"></param>
        public static void SaveMetadata(Repo repo, bool overwrite)
        {
            //Save Game Metadata
            foreach (var game in repo)
            {
                var gameDir = Path.Combine(repo.LocalPath, game.DirectoryName);
                var metadataPath = Path.Combine(gameDir, ".vngod");
                if (!File.Exists(metadataPath) || overwrite)
                {
                    using StreamWriter writer = new(metadataPath);
                    XmlSerializer serializer = new(typeof(Game));
                    serializer.Serialize(writer, game);
                    // Hide the .vngod file
                    //File.SetAttributes(metadataPath, File.GetAttributes(metadataPath) | FileAttributes.Hidden);
                }
            }
            // Save Repo Metadata
            var repoFilePath = Path.Combine(repo.LocalPath, ".vngodrepo");
            File.WriteAllText(repoFilePath, repo.RemotePath ?? "");
            // Hide the .vngodrepo file
            //File.SetAttributes(repoFilePath, File.GetAttributes(repoFilePath) | FileAttributes.Hidden);
        }
        /// <summary>
        /// Load metadata for all games in the repo from their .vngod files.
        /// </summary>
        /// <param name="repo"></param>
        public static void ReadMetadata(Repo repo)
        {
            for(int i=0; i < repo.Count; i++)
            {
                Game game = repo[i];
                var metaDataPath = Path.Combine(repo.LocalPath, game.DirectoryName, ".vngod");
                logger.Info("Reading metadata from " + metaDataPath);
                try
                {
                    using StreamReader reader = new(metaDataPath);
                    XmlSerializer serializer = new(typeof(Game));
                    repo[i] = (Game?)serializer.Deserialize(reader) ?? throw new Exception("Null game");
                }
                catch (Exception ex)
                {
                    Exception readEx = new($"Failed to read .vngod file in {metaDataPath}. Not a valid game directory.\n{ex.Message}");
                    logger.Error(readEx.Message, readEx);
                    throw readEx;
                }
            }
            
        }
        /// <summary>
        /// Read repo metadata from disk.
        /// </summary>
        /// <param name="repo"></param>
        /// <exception cref="Exception"></exception>
        private static void GetRepoInformation(Repo repo)
        {
            if (!File.Exists(Path.Combine(repo.LocalPath, ".vngodrepo")))
            {
                // If no .vngodrepo file exists, create one
                SaveMetadata(repo, false);
                return;
            }
            repo.RemotePath = File.ReadAllText(Path.Combine(repo.LocalPath, ".vngodrepo"));
        }
    }
}
