using System.Collections.Generic;
using System.Threading.Tasks;
using DaFiles.Models;

namespace DaFiles.Services.Repositories;

public sealed class DummyRepository : IRepository
{
    public static DummyRepository Instance { get; } = new DummyRepository();

    public async Task<Directory> ReadDirectoryAsync(string path)
    {
        return await Task.FromResult(new Directory(path, this, []));
    }

    public Task<List<DirectoryItem>> ReadDirectoryContentsAsync(string path)
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteItemsAsync(IEnumerable<DirectoryItem> items)
    {
        throw new System.NotImplementedException();
    }

    public string GetRootPath() => "/";

    public string CombinePath(string path1, string path2) => System.IO.Path.Combine(path1, path2);

    public void Dispose() { }

    public System.IO.Stream ReadFile(string path)
    {
        throw new System.NotImplementedException();
    }

    Task IRepository.WriteItemsAsync(IEnumerable<DirectoryItem> items, Directory source, Directory destination)
    {
        throw new System.NotImplementedException();
    }

    Task<bool> IRepository.MoveItemsWithinPlatformAsync(IEnumerable<DirectoryItem> items, Directory source, Directory destination)
    {
        throw new System.NotImplementedException();
    }

    Task<bool> IRepository.CopyItemsWithinPlatformAsync(IEnumerable<DirectoryItem> items, Directory source, Directory destination)
    {
        throw new System.NotImplementedException();
    }
}
