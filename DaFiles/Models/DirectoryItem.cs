using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace DaFiles.Models;

public enum DirectoryItemType
{
    Unknown,
    File,
    Folder,
}

public partial class DirectoryItem : ObservableObject
{
    public DirectoryItemType ItemType { get; }

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private DateTimeOffset? modifiedDate;

    [ObservableProperty]
    private string? extension;

    [ObservableProperty]
    private ulong? size;

    public DirectoryItem(DirectoryItemType itemType, string name, DateTimeOffset? modifiedDate, ulong? size)
    {
        ItemType = itemType;
        this.name = name;
        this.modifiedDate = modifiedDate;
        this.size = size;

        if (itemType == DirectoryItemType.File)
        {
            int nameDotIndex = name.LastIndexOf('.');
            if (nameDotIndex > -1)
                extension = name[(nameDotIndex + 1)..];
        }
    }
}
