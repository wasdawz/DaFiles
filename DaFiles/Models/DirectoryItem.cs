using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace DaFiles.Models;

public enum DirectoryItemType
{
    Unknown,
    File,
    Folder,
}

public partial class DirectoryItem(DirectoryItemType itemType, string name, DateTimeOffset? modifiedDate, string? extension, ulong? size) : ObservableObject
{
    public DirectoryItemType ItemType { get; } = itemType;

    [ObservableProperty]
    private string name = name;

    [ObservableProperty]
    private DateTimeOffset? modifiedDate = modifiedDate;

    [ObservableProperty]
    private string? extension = extension;

    [ObservableProperty]
    private ulong? size = size;
}
