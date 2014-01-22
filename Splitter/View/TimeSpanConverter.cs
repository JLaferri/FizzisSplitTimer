using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Fizzi.Applications.Splitter.Model;

namespace Fizzi.Applications.Splitter.View
{
    class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var timeSpan = value as TimeSpan?;

            if (timeSpan == null) return null;

            return Timer.FormatElapsedTimeSpan(timeSpan.Value, 7);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = value as string;

            if (string.IsNullOrWhiteSpace(str)) return null;

            try
            {
                var components = str.Split(':');

                //This code is to support specifying TimeSpan with hours greater than 23 instead of using days
                if (components.Length == 3)
                {
                    var hourComponent = int.Parse(components[0]);
                    int dayComponent = hourComponent / 24;

                    if (dayComponent > 0)
                    {
                        var sb = new StringBuilder();
                        sb.Append(dayComponent);
                        sb.Append(":");
                        sb.Append(hourComponent % 24);
                        sb.Append(":");
                        sb.Append(components[1]);
                        sb.Append(":");
                        sb.Append(components[2]);

                        str = sb.ToString();
                    }
                }
                else if (components.Length == 1) str = "0:0:" + str;
                else if (components.Length == 2) str = "0:" + str;

                return TimeSpan.Parse(str);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
