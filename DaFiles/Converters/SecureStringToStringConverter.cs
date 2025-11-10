using Avalonia.Data;
using Avalonia.Data.Converters;
using DaFiles.Helpers;
using System;
using System.Globalization;
using System.Security;

namespace DaFiles.Converters;

public class SecureStringToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        if (value is not SecureString secureString)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return secureString.Read();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        if (value is not string str)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        SecureString secureString = new();

        foreach (char ch in str)
        {
            secureString.AppendChar(ch);
        }

        return secureString;
    }
}
