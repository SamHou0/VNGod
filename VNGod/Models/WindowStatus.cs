using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNGod.Models
{
    class WindowStatus : INotifyPropertyChanged
    {
        private bool _isIdle = true;
        public bool IsIdle
        {
            get { return _isIdle; }
            set
            {
                _isIdle = value;
                OnPropertyChanged(nameof(IsIdle));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
