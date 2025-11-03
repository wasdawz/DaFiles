using CommunityToolkit.Mvvm.ComponentModel;
using DaFiles.Models;
using DaFiles.Services.Repositories;
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
            WebDavRepositoryConfig config = new(HostUrl, Username, Password, RootPath);
            WebDavRepository webDavRepository = new(config);
            repositoryModel = new("WebDAV", webDavRepository);
        }
        else
            throw new NotImplementedException();

        return repositoryModel;
    }
}
