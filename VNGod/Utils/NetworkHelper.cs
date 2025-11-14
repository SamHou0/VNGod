using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNGod.Data;
using VNGod.Network;

namespace VNGod.Utils
{
    static class NetworkHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(nameof(NetworkHelper));
        /// <summary>
        /// Get info from Bangumi using game directory name as keyword.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<bool> GetBangumiSubjectAsync(Game game, bool overwriteName)
        {
            try
            {
                Network.Bangumi.Datum datum = await Network.Bangumi.Api.PostSearchAsync(game.DirectoryName);
                DataParser.ParseBangumiSubject(datum, game, overwriteName);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Error fetching Bangumi info for {game.DirectoryName}: {ex.Message}", ex);
                return false;
            }
        }
        /// <summary>
        /// Get info from Bangumi using id (set in the game object).
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static async Task<bool> GetBangumiInfoAsync(Game game, bool overwriteName)
        {
            try
            {
                Network.Bangumi.Datum datum = await Network.Bangumi.Api.GetSubjectAsync(game.BangumiID ?? throw new Exception("ID not set"));
                DataParser.ParseBangumiSubject(datum, game, overwriteName);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Error fetching Bangumi info for ID {game.BangumiID}: {ex.Message}", ex);
                return false;
            }
        }
        /// <summary>
        /// Get info from VNDB using game directory name as keyword.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<bool> GetVNDBSubjectAsync(Game game, bool overwriteName)
        {
            try
            {
                Network.VNDB.Result result = await Network.VNDB.Api.PostSearchAsync(game.DirectoryName);
                DataParser.ParseVNDBResult(result, game, overwriteName);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Error fetching VNDB info for {game.DirectoryName}: {ex.Message}", ex);
                return false;
            }
        }
        /// <summary>
        /// Get info from VNDB using id (set in the game object).
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<bool> GetVNDBInfoAsync(Game game, bool overwriteName)
        {
            try
            {
                Network.VNDB.Result result = await Network.VNDB.Api.PostGetNameAsync(game.VNDBID ?? throw new Exception("ID not set"));
               DataParser.ParseVNDBResult(result, game, overwriteName);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Error fetching VNDB info for ID {game.VNDBID}: {ex.Message}", ex);
                return false;
            }
        }
    }
}
