using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using VNGod.Data;

namespace VNGod.Services
{
    static class FileService
    {
        /// <summary>
        /// Initializes a repo at the given path by reading its metadata and scanning for games.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Repo InitializeRepo(string path)
        {
            Repo repo = new Repo() { LocalPath = path };
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
                        using (StreamReader reader = new StreamReader(Path.Combine(dir, ".vngod")))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Game));
                            Game game = (Game?)serializer.Deserialize(reader) ?? throw new Exception("Null game");
                            repo.Add(game);
                        }
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
                catch
                {
                    throw new Exception($"Failed to read .vngod file in {dir}. Not a valid game directory.");
                }
            }
        }
        /// <summary>
        /// Save game and repo metadata to disk.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="overwrite"></param>
        public static void SaveMetadata(Repo repo, bool overwrite = false)
        {
            //Save Game Metadata
            foreach (var game in repo)
            {
                var gameDir = Path.Combine(repo.LocalPath, game.DirectoryName);
                var metadataPath = Path.Combine(gameDir, ".vngod");
                if (!File.Exists(metadataPath) || overwrite)
                {
                    using (StreamWriter writer = new StreamWriter(metadataPath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Game));
                        serializer.Serialize(writer, game);
                    }
                }
            }
            // Save Repo Metadata
            File.WriteAllText(Path.Combine(repo.LocalPath, ".vngodrepo"),
                repo.RemotePath ?? "");
        }
        /// <summary>
        /// Read repo metadata from disk.
        /// </summary>
        /// <param name="repo"></param>
        /// <exception cref="Exception"></exception>
        private static void GetRepoInformation(Repo repo)
        {
            if (!File.Exists(Path.Combine(repo.LocalPath, ".vngod")))
            {
                // If no .vngod file exists, create one
                SaveMetadata(repo);
                return;
            }
            repo.RemotePath = File.ReadAllText(Path.Combine(repo.LocalPath, ".vngod"));
        }
    }
}
