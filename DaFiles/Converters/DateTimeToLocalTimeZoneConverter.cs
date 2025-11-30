using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace DaFiles.Converters;

public class DateTimeToLocalTimeZoneConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dateTimeOffset)
            return dateTimeOffset.LocalDateTime;

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
