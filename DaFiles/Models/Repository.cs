using CommunityToolkit.Mvvm.ComponentModel;
using DaFiles.Services.Repositories;
using System;
using System.Threading.Tasks;

namespace DaFiles.Models;

public enum RemoteRepositoryType
{
    WebDav,
}

public sealed partial class Repository(RepositoryConfig config) : ObservableObject, IDisposable
{
    public RepositoryConfig Config { get; } = config;

    public IRepository Service { get; } = config.CreateService();

    public override string ToString() => Config.Name;

    public async Task TestConnectionOrThrowAsync()
    {
        await Service.ReadDirectoryContentsAsync(Service.GetRootPath());
    }

    public void Dispose() => Service.Dispose();
}
