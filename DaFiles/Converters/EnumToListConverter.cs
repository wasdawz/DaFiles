using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace DaFiles.Converters;

internal class EnumToListConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Type enumType)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return Enum.GetValues(enumType);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
