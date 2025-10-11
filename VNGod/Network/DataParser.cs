
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNGod.Data;

namespace VNGod.Network
{
    static class DataParser
    {
        public static Game ParseBangumiSubject(Bangumi.Datum datum,Game game,bool overwriteName)
        {
            if(overwriteName)
                game.Name = string.IsNullOrWhiteSpace(datum.name_cn) ? datum.name : datum.name_cn;//Use cn name first
            game.BangumiID = datum.id.ToString();
            return game;
        }
        public static Game ParseVNDBResult(VNDB.Result result,Game game,bool overwriteName)
        {
            var title = result.titles.FirstOrDefault(t => t.lang == "zh-Hans") ?? result.titles.FirstOrDefault();
            if (title != null&&overwriteName)
            {
                game.Name = title.title;
            }
            game.VNDBID = result.id;
            return game;
        }
    }
}
