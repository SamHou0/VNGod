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
        public static async Task<Game> GetBangumiInfoAsync(Game game)
        {
            try
            {
                Network.Bangumi.Datum datum = await Network.Bangumi.Api.PostSearchAsync(game.DirectoryName);
                return DataParser.ParseBangumiSubject(datum, game);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error fetching Bangumi info for {game.DirectoryName}: {ex.Message}");
            }
        }
    }
}
