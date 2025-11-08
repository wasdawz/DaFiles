using CommunityToolkit.Mvvm.ComponentModel;
using DaFiles.Models;
using System;
using System.Security;

namespace DaFiles.ViewModels;

public partial class RepositoryConnectViewModel : ViewModelBase
{
    [ObservableProperty]
    private RemoteRepositoryType repositoryType;

    [ObservableProperty]
    private string hostUrl = string.Empty;

    [ObservableProperty]
    private string? username;

    [ObservableProperty]
    private string? rootPath;

    [ObservableProperty]
    private string? errorMessage;

    public SecureString? Password { get; set; }

    /// <summary>
    /// Creates a <see cref="Repository"/> with config from the current view model.
    /// </summary>
    public Repository ToRepository()
    {
        Repository repositoryModel;

        if (RepositoryType == RemoteRepositoryType.WebDav)
        {
            WebDavRepositoryConfig config = new(Guid.NewGuid().ToString(), "WebDAV", HostUrl, Username, Password, RootPath);
            repositoryModel = new(config);
        }
        else
            throw new NotImplementedException();

        return repositoryModel;
    }
}
