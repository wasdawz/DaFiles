using DaFiles.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using WebDav;

namespace DaFiles.Services.Repositories;

public sealed class WebDavRepository : IRepository
{
    private readonly WebDavClient _client;
    private readonly WebDavRepositoryConfig _config;

    public WebDavRepository(WebDavRepositoryConfig config)
    {
        _config = config;

        WebDavClientParams clientParams = new()
        {
            BaseAddress = new(config.HostUrl),
            Credentials = new NetworkCredential(config.Username, config.Password),
        };

        _client = new WebDavClient(clientParams);
    }

    public async Task<Directory> ReadDirectoryAsync(string path)
    {
        var items = await ReadDirectoryContentsAsync(path);
        return new Directory(path, this, items);
    }

    public async Task<List<DirectoryItem>> ReadDirectoryContentsAsync(string path)
    {
        PropfindResponse result = await _client.Propfind(path);
        HandleWebDavResponse(result, "Failed to read directory", path);

        List<DirectoryItem> items = [];

        foreach (WebDavResource resource in result.Resources)
        {
            if (HttpUtility.UrlDecode(resource.Uri) is not string resourceUri ||
                resourceUri is null ||
                resourceUri == path ||
                (resourceUri.Length - path.Length == 1 && resourceUri.EndsWith('/')))
                continue;

            DirectoryItemType itemType = resource.IsCollection ? DirectoryItemType.Folder : DirectoryItemType.File;
            ulong? itemSize = (ulong?)resource.ContentLength;
            string itemName = resourceUri[path.Length..].Trim('/');

            items.Add(new(itemType, itemName, resource.LastModifiedDate, itemSize));
        }

        return items;
    }

    public async Task UploadItemsAsync(IEnumerable<DirectoryItem> items, IRepository sourceRepository, Directory sourceDirectory, Directory destination)
    {
        foreach (DirectoryItem item in items)
        {
            await UploadItemAsync(item, sourceRepository, sourceDirectory.Path, destination.Path);
        }
    }

    public System.IO.Stream ReadFile(string path)
    {
        throw new NotImplementedException();
    }

    public string GetRootPath() => _config.RootPath ?? string.Empty;

    public string CombinePath(string path1, string path2)
    {
        return $"{path1.TrimEnd('/')}/{path2.TrimStart('/')}";
    }

    public void Dispose() => _client.Dispose();

    async Task IRepository.WriteItemsAsync(IEnumerable<DirectoryItem> items, Directory source, Directory destination)
    {
        await UploadItemsAsync(items, source.Repository, source, destination);
    }

    Task<bool> IRepository.MoveItemsWithinPlatformAsync(IEnumerable<DirectoryItem> items, Directory source, Directory destination)
    {
        throw new NotImplementedException();
    }

    Task<bool> IRepository.CopyItemsWithinPlatformAsync(IEnumerable<DirectoryItem> items, Directory source, Directory destination)
    {
        throw new NotImplementedException();
    }

    private async Task UploadItemAsync(DirectoryItem item, IRepository sourceRepository, string sourceFolderPath, string destinationFolderPath)
    {
        string itemPath = sourceRepository.CombinePath(sourceFolderPath, item.Name);

        if (item.ItemType == DirectoryItemType.Folder)
            await UploadFolderAsync(item, itemPath, sourceRepository, destinationFolderPath);
        else if (item.ItemType == DirectoryItemType.File)
            await UploadFileAsync(item, itemPath, sourceRepository, destinationFolderPath);
        else
            throw new ArgumentException("Unsupported item type.");
    }

    private async Task UploadFolderAsync(DirectoryItem folderItem, string folderItemPath, IRepository sourceRepository, string destinationFolderPath)
    {
        string newDestinationFolderPath = CombinePath(destinationFolderPath, folderItem.Name);

        WebDavResponse response = await _client.Mkcol(newDestinationFolderPath);

        // Skip error caused by directory already existing.
        if (response.StatusCode != 405)
            HandleWebDavResponse(response, "Failed to create directory", newDestinationFolderPath);

        foreach (DirectoryItem item in await sourceRepository.ReadDirectoryContentsAsync(folderItemPath))
        {
            await UploadItemAsync(item, sourceRepository, folderItemPath, newDestinationFolderPath);
        }
    }

    private async Task UploadFileAsync(DirectoryItem fileItem, string fileItemPath, IRepository sourceRepository, string destinationFolderPath)
    {
        string destinationFilePath = CombinePath(destinationFolderPath, fileItem.Name);
        Dictionary<string, string> headers = [];

        if (fileItem.ModifiedDate is DateTimeOffset modifiedDate)
            headers.Add("X-OC-MTime", modifiedDate.ToUnixTimeSeconds().ToString());

        PutFileParameters parameters = new()
        {
            Headers = headers,
        };

        using System.IO.Stream sourceStream = sourceRepository.ReadFile(fileItemPath);
        WebDavResponse response = await _client.PutFile(destinationFilePath, sourceStream, parameters);
        HandleWebDavResponse(response, "Failed to upload file", destinationFilePath);
    }

    private static void HandleWebDavResponse(WebDavResponse response, string errorMessage, string? messageParameter = null)
    {
        if (!response.IsSuccessful)
        {
            string message = errorMessage;
            if (!string.IsNullOrEmpty(messageParameter))
                message += $" {messageParameter}";
            throw new WebException($"{message} ({response.StatusCode} {response.Description})");
        }
    }
}
