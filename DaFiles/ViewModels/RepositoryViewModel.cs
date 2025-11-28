using CommunityToolkit.Mvvm.Input;
using DaFiles.Helpers;
using DaFiles.Models;
using System;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public partial class RepositoryViewModel : ViewModelBase
{
    public Repository Repository { get; }

    public CachingAsyncNavigationStack<string, DirectoryViewModel> DirectoryNavigation { get; }

    public DirectoryViewModel? CurrentDirectory => DirectoryNavigation.CurrentItem;

    public string? ErrorMessage => _errorMessage;

    private string? _errorMessage;
    private DirectoryViewModel? _directoryWithErrorMessage;

    public RepositoryViewModel(Repository repository)
    {
        Repository = repository;
        DirectoryNavigation = new(ReadDirectory);
        DirectoryNavigation.CurrentItemChanged += OnCurrentDirectoryChanged;
        DirectoryNavigation.LoadingCachedItemFailed += DirectoryNavigation_LoadingCachedItemFailed;
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
        DirectoryViewModel? directoryViewModel = CurrentDirectory;

        if (directoryViewModel is null)
        {
            if (!await TryEnsureInitializedAsync())
                return;

            directoryViewModel = CurrentDirectory;

            if (directoryViewModel is null)
                return;
        }

        try
        {
            await directoryViewModel.Directory.RefreshAsync();
        }
        catch (Exception ex)
        {
            SetError(ex.Message, directoryViewModel);
        }
    }

    public override string ToString() => Repository.Config.Name;

    private void SetError(string? message, DirectoryViewModel? directory)
    {
        _errorMessage = message;
        _directoryWithErrorMessage = directory;
        OnPropertyChanged(nameof(ErrorMessage));
    }

    [RelayCommand]
    private void ClearMessage()
    {
        SetError(null, null);
    }

    private void OnCurrentDirectoryChanged()
    {
        if (_errorMessage is not null && CurrentDirectory != _directoryWithErrorMessage)
            ClearMessage();
    }

    private void DirectoryNavigation_LoadingCachedItemFailed(LoadingCachedItemFailedEventArgs<DirectoryViewModel> e)
    {
        if (e.Error is not null)
            SetError(e.Error.Message, e.Item);
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
            SetError(ex.Message, CurrentDirectory);
            return null;
        }
    }
}
