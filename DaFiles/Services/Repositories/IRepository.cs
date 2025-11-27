using DaFiles.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DaFiles.Services.Repositories;

public interface IRepository : IDisposable
{
    string GetRootPath();

    string CombinePath(string path1, string path2);

    Task<Directory> ReadDirectoryAsync(string path);

    Task<List<DirectoryItem>> ReadDirectoryContentsAsync(string path);

    System.IO.Stream ReadFile(string path);
}
