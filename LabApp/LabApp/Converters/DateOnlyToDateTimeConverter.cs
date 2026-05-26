using System;
using System.Globalization;
using System.Windows.Data;

namespace LabApp.WPF.Converters;

public class DateOnlyToDateTimeConverter : IValueConverter
{
    // Из DateOnly (БД/Модель) в DateTime (DatePicker)
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateOnly dateOnly)
            return dateOnly.ToDateTime(TimeOnly.MinValue);

        return null;
    }

    // Из DateTime (DatePicker) обратно в DateOnly (БД/Модель)
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
            return DateOnly.FromDateTime(dateTime);

        return null;
    }
}