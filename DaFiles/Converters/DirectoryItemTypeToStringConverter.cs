using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;
using DaFiles.Models;

namespace DaFiles.Converters;

public class DirectoryItemTypeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        if (value is not DirectoryItem directoryItem)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        if (directoryItem.ItemType == DirectoryItemType.Folder)
        {
            return "Folder";
        }
        else if (directoryItem.ItemType == DirectoryItemType.File)
        {
            if (directoryItem.Extension is string extension)
                return $"{extension.ToUpper()} file";
            else
                return "File";
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
