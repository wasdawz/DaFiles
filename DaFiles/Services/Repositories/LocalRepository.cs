using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaFiles.Models;

namespace DaFiles.Services.Repositories;

public class LocalRepository : IRepository
{
    public Func<TopLevel?>? TopLevelGetter { private get; set; }

    public async Task<Directory> ReadDirectoryAsync(string path)
    {
        var items = await ReadDirectoryContentsAsync(path);
        return new Directory(path, this, items);
    }

    public async Task<List<DirectoryItem>> ReadDirectoryContentsAsync(string path)
    {
        var storage = GetStorageProvider();

        if (await storage.TryGetFolderFromPathAsync(path) is not IStorageFolder folder)
            throw new ArgumentException("Local folder at this path doesn't exist.");

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
            string? extension = null;

            if (itemType == DirectoryItemType.File)
            {
                int nameDotIndex = item.Name.LastIndexOf('.');
                if (nameDotIndex > -1)
                    extension = item.Name[(nameDotIndex + 1)..];
            }

            items.Add(new(itemType, item.Name, itemProperties.DateModified, extension, itemSize));
        }

        return items;
    }

    public string GetRootPath() => "E:\\";

    public string CombinePath(string path1, string path2) => System.IO.Path.Combine(path1, path2);

    private IStorageProvider GetStorageProvider()
    {
        if (TopLevelGetter?.Invoke() is not TopLevel topLevel)
            throw new InvalidOperationException("TopLevel not provided.");

        return topLevel.StorageProvider;
    }
}
