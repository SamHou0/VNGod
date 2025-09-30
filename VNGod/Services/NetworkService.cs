using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNGod.Data;
using VNGod.Network;

namespace VNGod.Services
{
    static class NetworkService
    {
        public static async Task<Game> GetBangumiSubjectAsync(Game game)
        {
            try
            {
                Network.Bangumi.Datum datum = await Network.Bangumi.Api.PostSearchAsync(game.DirectoryName);
                return DataParser.ParseBangumiSubject(datum, game);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching Bangumi info for {game.DirectoryName}: {ex.Message}");
            }
        }
        /// <summary>
        /// Get Bangumi info using id (set in the game object).
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static async Task<Game> GetBangumiInfoAsync(Game game)
        {
            try
            {
                Network.Bangumi.Datum datum = await Network.Bangumi.Api.GetSubjectAsync(game.BangumiID ?? throw new Exception("ID not set"));
                return DataParser.ParseBangumiSubject(datum, game);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching Bangumi info for ID {game.BangumiID}: {ex.Message}");
            }
        }
    }
}
