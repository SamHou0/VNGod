using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles
namespace VNGod.Network.VNDB
{
    public class SearchModel
    {
        public string[] filters { get; set; }
        public string fields { get; set; }
    }

    public class SearchResult
    {
        public bool more { get; set; }
        public Result[] results { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public Title[] titles { get; set; }
    }

    public class Title
    {
        public string lang { get; set; }
        public string title { get; set; }
    }


}
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
