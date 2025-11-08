using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using DaFiles.Services.Repositories;

namespace DaFiles.Models;

public enum RemoteRepositoryType
{
    WebDav,
}

public partial class Repository(RepositoryConfig config) : ObservableObject
{
    public RepositoryConfig Config { get; } = config;

    public IRepository Service { get; } = config.CreateService();

    public override string ToString() => Config.Name;

    public async Task TestConnectionOrThrowAsync()
    {
        await Service.ReadDirectoryContentsAsync(Service.GetRootPath());
    }
}
