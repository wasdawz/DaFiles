using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaFiles.Models;

namespace DaFiles.Services.Repositories;

public sealed class LocalRepository(Func<TopLevel?> topLevelGetter) : IRepository
{
    private readonly Func<TopLevel?> _topLevelGetter = topLevelGetter;

    public async Task<Directory> ReadDirectoryAsync(string path)
    {
        IStorageFolder folder = await GetFolderFromPathAsync(path);
        List<DirectoryItem> items = await ReadDirectoryContentsAsync(folder);
        string actualPath = folder.Path.IsAbsoluteUri
            ? folder.Path.LocalPath.TrimEnd('\\')
            : folder.Path.OriginalString;
        return new Directory(actualPath, this, items);
    }

    public async Task<List<DirectoryItem>> ReadDirectoryContentsAsync(string path)
    {
        IStorageFolder folder = await GetFolderFromPathAsync(path);
        return await ReadDirectoryContentsAsync(folder);
    }

    public async Task MoveItemsAsync(IEnumerable<DirectoryItem> items, Directory destination)
    {
        if (destination.Repository is not LocalRepository)
            throw new ArgumentException("Destination directory is not local.");

        IStorageProvider storage = GetStorageProvider();

        if (await storage.TryGetFolderFromPathAsync(destination.Path) is not IStorageFolder destinationFolder)
            throw new ArgumentException("Destination folder doesn't exist.");

        foreach (DirectoryItem item in items)
        {
            if (item.PlatformObject is not IStorageItem storageItem)
                throw new ArgumentException("Local item is missing the platform object.");

            await storageItem.MoveAsync(destinationFolder);
        }
    }

    public string GetRootPath() => "\\";

    public string CombinePath(string path1, string path2) => System.IO.Path.Combine(path1, path2);

    private static async Task<List<DirectoryItem>> ReadDirectoryContentsAsync(IStorageFolder folder)
    {
        List<DirectoryItem> items = [];

        await foreach (var item in folder.GetItemsAsync())
        {
            DirectoryItemType itemType = item switch
            {
                IStorageFile => DirectoryItemType.File,
                IStorageFolder => DirectoryItemType.Folder,
                _ => DirectoryItemType.Unknown,
            };

            var itemProperties = await item.GetBasicPropertiesAsync();
            var itemSize = itemType == DirectoryItemType.File ? itemProperties.Size : null;

            DirectoryItem directoryItem = new(itemType, item.Name, itemProperties.DateModified, itemSize)
            {
                PlatformObject = item,
            };

            items.Add(directoryItem);
        }

        return items;
    }

    private async Task<IStorageFolder> GetFolderFromPathAsync(string path)
    {
        IStorageProvider storage = GetStorageProvider();
        return await storage.TryGetFolderFromPathAsync(path)
            ?? throw new ArgumentException("Local folder at this path doesn't exist.");
    }

    private IStorageProvider GetStorageProvider()
    {
        if (_topLevelGetter() is not TopLevel topLevel)
            throw new InvalidOperationException("TopLevel not provided.");

        return topLevel.StorageProvider;
    }

    public void Dispose() { }
}
