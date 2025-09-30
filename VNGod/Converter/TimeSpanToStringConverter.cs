using System;
using System.Globalization;
using System.Windows.Data;

namespace VNGod.Converter
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        // TimeSpan -> string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
            {
                int totalHours = (int)ts.TotalHours;
                return $"{totalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}";
            }
            return "";
        }

        // string -> TimeSpan
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Extract hours, minutes, seconds from string formatted as "H:MM:SS"
            if (value is string s)
            {
                var parts = s.Split(':');
                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int hours) &&
                    int.TryParse(parts[1], out int minutes) &&
                    int.TryParse(parts[2], out int seconds))
                {
                    if (minutes >= 0 && minutes < 60 && seconds >= 0 && seconds < 60 && hours >= 0)
                        return new TimeSpan(hours, minutes, seconds);
                }
            }
            return TimeSpan.Zero;
        }
    }
}