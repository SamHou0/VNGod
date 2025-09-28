using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNGod.Data
{
    public class Repo : ObservableCollection<Game>
    {
        public required string LocalPath { get; set; }
        public string? RemotePath { get; set; } = null;
    }
}
