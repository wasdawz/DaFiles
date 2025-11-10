using CommunityToolkit.Mvvm.ComponentModel;
using DaFiles.Models;
using System;
using System.Security;

namespace DaFiles.ViewModels;

public partial class RepositoryConfigViewModel : ViewModelBase
{
    [ObservableProperty]
    private RemoteRepositoryType repositoryType;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string hostUrl = string.Empty;

    [ObservableProperty]
    private string? username;

    [ObservableProperty]
    private string? rootPath;

    [ObservableProperty]
    private string? errorMessage;

    // Not observable in order to reduce unnecessary conversions when bound in TwoWay mode.
    public SecureString? Password { get; set; }

    public string? Id { get; }

    public RepositoryConfigViewModel()
    {
    }

    /// <summary>
    /// Initializes an instance with values from the specified <paramref name="repositoryConfig"/>.
    /// </summary>
    /// <param name="repositoryConfig"><see cref="RepositoryConfig"/> to take values from.</param>
    /// <param name="preserveId">Whether the configs created by this instance should preserve the ID
    /// of the specified <paramref name="repositoryConfig"/>.</param>
    public RepositoryConfigViewModel(RepositoryConfig repositoryConfig, bool preserveId = false)
    {
        if (preserveId)
            Id = repositoryConfig.Id;

        if (repositoryConfig is WebDavRepositoryConfig webDavConfig)
        {
            RepositoryType = RemoteRepositoryType.WebDav;
            Name = webDavConfig.Name;
            HostUrl = webDavConfig.HostUrl;
            Username = webDavConfig.Username;
            Password = webDavConfig.Password;
            RootPath = webDavConfig.RootPath;
        }
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a <see cref="Repository"/> with config from the current view model.
    /// </summary>
    public Repository ToRepository()
    {
        Repository repositoryModel;
        string id = Id ?? Guid.NewGuid().ToString();

        if (RepositoryType == RemoteRepositoryType.WebDav)
        {
            WebDavRepositoryConfig config = new(id, Name, HostUrl, Username, Password, RootPath);
            repositoryModel = new(config);
        }
        else
            throw new NotImplementedException();

        return repositoryModel;
    }
}
