using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNGod.Data
{
    public class Game : INotifyPropertyChanged
    {
        private string? _name;
        private string? _savePath;//Detimine the path of the save files of a game
        private string? _executableName;//Detemine the path of the executable file of a game
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
            if (string.IsNullOrEmpty(Name)) return "[Unknown] " + DirectoryName;
            else return Name;
        }
        public string ShortDescription
        {
            get
            {
                // Same as tostring
                if (string.IsNullOrEmpty(Name)) return "[Unknown] " + DirectoryName;
                else return Name;
            }
        }
        public string Description
        {
            get
            {
                return $"Cloud Save Path: {(string.IsNullOrEmpty(SavePath) ? "[Not Set]" : SavePath)}" + Environment.NewLine +
                    $"Name: {(string.IsNullOrEmpty(Name) ? "[Unknown]" : Name)}"
                    + Environment.NewLine + $"Directory Name: {DirectoryName}" + Environment.NewLine +
                    $"Executable Name: {(string.IsNullOrEmpty(ExecutableName) ? "[Not Set]" : ExecutableName)}" + Environment.NewLine +
                    $"Process Name: {(string.IsNullOrEmpty(ProcessName) ? "[Not Set]" : ProcessName)}" + Environment.NewLine +
                    $"Play Time: {(int)PlayTime.TotalHours}:{PlayTime.Minutes:D2}:{PlayTime.Seconds:D2}" + Environment.NewLine +
                    $"Bangumi ID: {(string.IsNullOrEmpty(BangumiID) ? "[Unknown]" : BangumiID)}" +
                    Environment.NewLine +
                    $"VNDB ID: {(string.IsNullOrEmpty(VNDBID) ? "[Unknown]" : VNDBID)}";
            }
        }
    }
}
