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

    public async Task CopyItemsAsync(IEnumerable<DirectoryItem> items, Directory destination)
    {
        if (destination.Repository is not LocalRepository)
            throw new ArgumentException("Destination directory is not local.");

        IStorageProvider storage = GetStorageProvider();

        if (await storage.TryGetFolderFromPathAsync(destination.Path) is null)
            throw new ArgumentException("Destination folder doesn't exist.");

        foreach (DirectoryItem item in items)
        {
            if (item.PlatformObject is not IStorageItem storageItem)
                throw new ArgumentException("Local item has no valid platform object.");

            await CopyItemAsync(storageItem, destination.Path);
        }
    }

    public string GetRootPath() => "\\";

    public string CombinePath(string path1, string path2) => System.IO.Path.Combine(path1, path2);

    private async Task CopyItemAsync(IStorageItem item, string destinationFolderPath)
    {
        if (item is IStorageFolder storageFolder)
            await CopyFolderAsync(storageFolder, destinationFolderPath);
        else if (item is IStorageFile storageFile)
            await CopyFileAsync(storageFile, destinationFolderPath);
        else
            throw new ArgumentException("Local item has an invalid platform object.");
    }

    private async Task CopyFolderAsync(IStorageFolder storageFolder, string destinationFolderPath)
    {
        string newFolderPath = CombinePath(destinationFolderPath, storageFolder.Name);
        System.IO.Directory.CreateDirectory(newFolderPath);

        await foreach (IStorageItem item in storageFolder.GetItemsAsync())
        {
            await CopyItemAsync(item, newFolderPath);
        }
    }

    private async Task CopyFileAsync(IStorageFile sourceFile, string destinationFolderPath)
    {
        string destinationFilePath = CombinePath(destinationFolderPath, sourceFile.Name);
        try
        {
            using (System.IO.FileStream sourceStream = System.IO.File.OpenRead(sourceFile.Path.AbsolutePath))
            {
                using System.IO.FileStream destinationStream = new(destinationFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                await sourceStream.CopyToAsync(destinationStream);
            }
            DateTime fileModifiedDate = System.IO.File.GetLastWriteTimeUtc(sourceFile.Path.AbsolutePath);
            System.IO.File.SetLastWriteTimeUtc(destinationFilePath, fileModifiedDate);
        }
        catch
        {
            if (System.IO.File.Exists(destinationFilePath))
                System.IO.File.Delete(destinationFilePath);

            throw;
        }
    }

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
