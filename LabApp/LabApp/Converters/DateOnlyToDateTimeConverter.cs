using System;
using System.Globalization;
using System.Windows.Data;

namespace LabApp.WPF.Converters
{
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly date)
                return date.ToDateTime(TimeOnly.MinValue);
            return DateTime.Today;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return DateOnly.FromDateTime(dt);
            return DateOnly.FromDateTime(DateTime.Today);
        }
    }
}