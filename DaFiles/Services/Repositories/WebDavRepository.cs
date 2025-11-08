using DaFiles.Models;
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

        if (!result.IsSuccessful)
            throw new WebException($"Failed to read directory ({result.StatusCode} {result.Description})");

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

    public string GetRootPath() => _config.RootPath ?? string.Empty;

    public string CombinePath(string path1, string path2)
    {
        return $"{path1.TrimEnd('/')}/{path2.TrimStart('/')}";
    }

    public void Dispose() => _client.Dispose();
}
