using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using DaFiles.Services.Repositories;

namespace DaFiles.Models;

public enum RemoteRepositoryType
{
    WebDav,
}

public partial class Repository(string name, IRepository service) : ObservableObject
{
    [ObservableProperty]
    private string name = name;

    public IRepository Service { get; } = service;

    public override string ToString() => Name;

    public async Task TestConnectionOrThrowAsync()
    {
        await Service.ReadDirectoryContentsAsync(Service.GetRootPath());
    }
}
