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

    public DirectoryViewModel? CurrentDirectory => DirectoryNavigation.CurrentItem;

    [ObservableProperty]
    private string? errorMessage;

    public RepositoryViewModel(Repository repository)
    {
        Repository = repository;
        DirectoryNavigation = new(ReadDirectory);
        DirectoryNavigation.CurrentItemChanged += ClearMessage;
        DirectoryNavigation.LoadingCachedItem += DirectoryNavigation_LoadingCachedItem;
    }

    public async Task<bool> TryEnsureInitializedAsync()
    {
        if (CurrentDirectory is null)
        {
            await DirectoryNavigation.SetTopItemAsync(Repository.Service.GetRootPath());
            return CurrentDirectory is not null;
        }
        else
            return true;
    }

    public async Task OpenItemAsync(object? item)
    {
        if (item is not DirectoryItem directoryItem ||
            CurrentDirectory is not DirectoryViewModel currentDirectory)
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
        Directory? directory = CurrentDirectory?.Directory;

        if (directory is null)
        {
            if (!await TryEnsureInitializedAsync())
                return;

            directory = CurrentDirectory?.Directory;

            if (directory is null)
                return;
        }

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

    private async void DirectoryNavigation_LoadingCachedItem(LoadingCachedItemEventArgs<DirectoryViewModel> e)
    {
        try
        {
            await e.Item.Directory.RefreshAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            e.Cancel = true;
        }
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
