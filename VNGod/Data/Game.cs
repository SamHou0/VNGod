using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNGod.Data
{
    class Game
    {
        public required string DirectoryName { get; set; }
        public string? Name { get; set; }
        public string? SavePath { get; set; }
        public string? BangumiID { get; set; }
        public string? VNDBID { get; set; }
        public TimeSpan PlayTime { get; set; } = TimeSpan.Zero;
        public override string ToString()
        {
            // Display Name if exists, otherwise DirectoryName
            if (string.IsNullOrEmpty(Name)) return "[Unknown] " + DirectoryName;
            else return Name;
        }
        public string Description
        {
            get
            {
                return $"Cloud Save Path: {(string.IsNullOrEmpty(SavePath) ? "[Not Set]" : SavePath)}" + Environment.NewLine +
                    $"Name: {(string.IsNullOrEmpty(Name) ? "[Unknown]" : Name)}"
                    + Environment.NewLine + $"Directory Name: {DirectoryName}" +Environment.NewLine+
                    $"Play Time: {PlayTime}" + Environment.NewLine +
                    $"Bangumi ID: {(string.IsNullOrEmpty(BangumiID) ? "[Unknown]" : BangumiID)}" +
                    Environment.NewLine +
                    $"VNDB ID: {(string.IsNullOrEmpty(VNDBID) ? "[Unknown]" : VNDBID)}";
            }
        }
    }
}
