using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using DaFiles.Helpers;
using DaFiles.Models;

namespace DaFiles.ViewModels;

public partial class RepositoryViewModel : ViewModelBase
{
    public Repository Repository { get; }

    public CachingAsyncNavigationStack<string, DirectoryViewModel> DirectoryNavigation { get; }

    [ObservableProperty]
    private string? errorMessage;

    private bool _isInitialized = false;

    public RepositoryViewModel(Repository repository)
    {
        Repository = repository;
        DirectoryNavigation = new(ReadDirectory);
        DirectoryNavigation.CurrentItemChanged += ClearMessage;
    }

    public async Task EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            await DirectoryNavigation.SetTopItemAsync(Repository.Service.GetRootPath());
            _isInitialized = true;
        }
    }

    public DirectoryViewModel? GetCurrentDirectory() => DirectoryNavigation.CurrentItem;

    public async Task OpenItemAsync(object? item)
    {
        if (item is not DirectoryItem directoryItem ||
            GetCurrentDirectory() is not DirectoryViewModel currentDirectory)
            return;

        if (directoryItem.ItemType == DirectoryItemType.Folder)
        {
            string path = Repository.Service.CombinePath(currentDirectory.Directory.Path, directoryItem.Name);
            await DirectoryNavigation.SetTopItemAsync(path);
        }
    }

    [RelayCommand]
    public async Task RefreshCurrentDirectoryAsync()
    {
        if (GetCurrentDirectory()?.Directory is not Directory directory)
            return;

        try
        {
            await directory.RefreshAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public override string ToString() => Repository.Config.Name;

    [RelayCommand]
    private void ClearMessage()
    {
        ErrorMessage = null;
    }

    private async Task<DirectoryViewModel?> ReadDirectory(string path)
    {
        try
        {
            Directory directory = await Repository.Service.ReadDirectoryAsync(path);
            return new(directory);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return null;
        }
    }
}
