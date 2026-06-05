using System;
using System.Globalization;
using System.Windows.Data;

namespace BeautySalonWpf
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan time)
            {
                return time.ToString(@"hh\:mm"); // Форматируем временной интервал
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();


        }
    }
}