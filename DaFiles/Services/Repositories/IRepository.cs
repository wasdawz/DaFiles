using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaFiles.Models;

namespace DaFiles.Services.Repositories;

public interface IRepository : IDisposable
{
    public string GetRootPath();
    public string CombinePath(string path1, string path2);
    public Task<Directory> ReadDirectoryAsync(string path);
    public Task<List<DirectoryItem>> ReadDirectoryContentsAsync(string path);
}
