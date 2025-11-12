using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;
using VNGod.Resource.Strings;

namespace VNGod.Data
{
    public class Game : INotifyPropertyChanged
    {
        private string? _name;
        private string? _savePath;//Detimine the path of the save files of a game
        private string? _executableName;//Detemine the path of the executable file of a game
        private ImageSource? _icon;//Icon of the game
        private string? _processName;//Detemine the process name of a game, for time recording purpose
        private string? _bangumiID;
        private string? _vndbID;
        private TimeSpan _playTime = TimeSpan.Zero;
        public required string DirectoryName { get; set; }
        public string? Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(ShortDescription));
            }
        }
        public string? SavePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                OnPropertyChanged(nameof(SavePath));
            }
        }
        public string? ExecutableName
        {
            get { return _executableName; }
            set
            {
                _executableName = value;
                OnPropertyChanged(nameof(ExecutableName));
            }
        }
        [XmlIgnore]
        public ImageSource? Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
        public string? ProcessName
        {
            get { return _processName; }
            set
            {
                _processName = value;
                OnPropertyChanged(nameof(ProcessName));
            }
        }
        public string? BangumiID
        {
            get { return _bangumiID; }
            set
            {
                _bangumiID = value;
                OnPropertyChanged(nameof(BangumiID));
            }
        }
        public string? VNDBID
        {
            get { return _vndbID; }
            set
            {
                _vndbID = value;
                OnPropertyChanged(nameof(VNDBID));
            }
        }
        public TimeSpan PlayTime
        {
            get { return _playTime; }
            set
            {
                _playTime = value;
                OnPropertyChanged(nameof(PlayTime));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
        }

        public override string ToString()
        {
            // Display Name if exists, otherwise DirectoryName
            if (string.IsNullOrEmpty(Name)) return Strings.Unknown + DirectoryName;
            else return Name;
        }
        public string ShortDescription
        {
            get
            {
                // Same as tostring
                if (string.IsNullOrEmpty(Name)) return Strings.Unknown + DirectoryName;
                else return Name;
            }
        }
        public string Description
        {
            get
            {
                return $"{Strings.SavePath} {(string.IsNullOrEmpty(SavePath) ? Strings.NotSet : SavePath)}" + Environment.NewLine +
                    $"{Strings.Name} {(string.IsNullOrEmpty(Name) ? Strings.NotSet : Name)}"
                    + Environment.NewLine + $"{Strings.DirectoryName} {DirectoryName}" + Environment.NewLine +
                    $"{Strings.ExecutableName} {(string.IsNullOrEmpty(ExecutableName) ? Strings.NotSet : ExecutableName)}" + Environment.NewLine +
                    $"{Strings.ProcessName} {(string.IsNullOrEmpty(ProcessName) ? Strings.NotSet : ProcessName)}" + Environment.NewLine +
                    $"{Strings.PlayTime} {(int)PlayTime.TotalHours}:{PlayTime.Minutes:D2}:{PlayTime.Seconds:D2}" + Environment.NewLine +
                    $"{Strings.BangumiID} {(string.IsNullOrEmpty(BangumiID) ? Strings.NotSet : BangumiID)}" +
                    Environment.NewLine +
                    $"{Strings.VNDBID} {(string.IsNullOrEmpty(VNDBID) ? Strings.NotSet : VNDBID)}";
            }
        }
    }
}
