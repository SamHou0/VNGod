using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNGod.Network.Bangumi
{
    public class SearchModel
    {
        public required string keyword { get; set; }
        public Filter filter { get; set; } = new();
    }

    public class Filter
    {
        public int[] type { get; set; } = [4];
    }
#pragma warning disable 8618 // Disable non-nullable field must contain a non-null value when exiting constructor.
    public class SearchResult
    {
        public Datum[] data { get; set; }
        public int total { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
    }

    public class Datum
    {
        public string date { get; set; }
        public string platform { get; set; }
        public Images images { get; set; }
        public string image { get; set; }
        public string summary { get; set; }
        public string name { get; set; }
        public string name_cn { get; set; }
        public Tag[] tags { get; set; }
        public Infobox[] infobox { get; set; }
        public Rating rating { get; set; }
        public Collection collection { get; set; }
        public int id { get; set; }
        public int eps { get; set; }
        public string[] meta_tags { get; set; }
        public int volumes { get; set; }
        public bool series { get; set; }
        public bool locked { get; set; }
        public bool nsfw { get; set; }
        public int type { get; set; }
    }

    public class Images
    {
        public string small { get; set; }
        public string grid { get; set; }
        public string large { get; set; }
        public string medium { get; set; }
        public string common { get; set; }
    }

    public class Rating
    {
        public int rank { get; set; }
        public int total { get; set; }
        public Count count { get; set; }
        public float score { get; set; }
    }

    public class Count
    {
        public int _1 { get; set; }
        public int _2 { get; set; }
        public int _3 { get; set; }
        public int _4 { get; set; }
        public int _5 { get; set; }
        public int _6 { get; set; }
        public int _7 { get; set; }
        public int _8 { get; set; }
        public int _9 { get; set; }
        public int _10 { get; set; }
    }

    public class Collection
    {
        public int on_hold { get; set; }
        public int dropped { get; set; }
        public int wish { get; set; }
        public int collect { get; set; }
        public int doing { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public int count { get; set; }
        public int total_cont { get; set; }
    }

    public class Infobox
    {
        public string key { get; set; }
        public object value { get; set; }
    }
#pragma warning restore 8618
}
