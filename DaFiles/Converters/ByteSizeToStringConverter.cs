using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace DaFiles.Converters;

public class ByteSizeToStringConverter : IValueConverter
{
    private static readonly NumberFormatInfo NumberFormat = new()
    {
        NumberGroupSeparator = " ",
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        if (value is not ulong byteSize)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        var kilobytes = byteSize / 1024;
        if (byteSize % 1024 > 0)
            kilobytes += 1;
        return kilobytes.ToString("#,0", NumberFormat) + " KB";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
