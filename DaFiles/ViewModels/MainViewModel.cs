using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaFiles.Models;
using DaFiles.Services;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isSecondNavigationPaneOpen;

    public NavigationPaneViewModel FirstNavigationPane { get; }
    public NavigationPaneViewModel SecondNavigationPane { get; }

    public ReadOnlyObservableCollection<Repository> EditableRepositories { get; }

    private readonly SourceList<Repository> _repositories;
    private readonly IDataStore<Repository> _repositoryStore;

    public MainViewModel(IEnumerable<Repository> repositories, IDataStore<Repository> repositoryStore)
    {
        _repositories = new();
        _repositories.AddRange(repositories);
        _repositoryStore = repositoryStore;
        FirstNavigationPane = new(_repositories);
        SecondNavigationPane = new(_repositories);
        _repositories.Connect().Filter(r => r.Config.IsSaveable).Bind(out var editableRepositories).Subscribe();
        EditableRepositories = editableRepositories;
    }

    [RelayCommand]
    public void ToggleSecondPane()
    {
        IsSecondNavigationPaneOpen = !IsSecondNavigationPaneOpen;
    }

    public async Task LoadAsync()
    {
        await FirstNavigationPane.LoadAsync();
        await SecondNavigationPane.LoadAsync();
    }

    public async Task<bool> TryAddNewRepositoryAsync(RepositoryConfigViewModel configViewModel)
    {
        if (await CreateRepositoryOrSetErrorAsync(configViewModel) is not Repository repository)
            return false;

        _repositories.Add(repository);

        await _repositoryStore.SaveAsync(_repositories.Items);
        return true;
    }

    public async Task<bool> TryUpdateRepositoryConfigAsync(Repository repositoryToUpdate, RepositoryConfigViewModel configViewModel)
    {
        int repositoryIndex = _repositories.Items.IndexOf(repositoryToUpdate);

        if (repositoryIndex < 0 ||
            await CreateRepositoryOrSetErrorAsync(configViewModel) is not Repository newRepository)
        {
            return false;
        }

        _repositories.ReplaceAt(repositoryIndex, newRepository);
        repositoryToUpdate.Dispose();
        await _repositoryStore.SaveAsync(_repositories.Items);
        return true;
    }

    public async Task DeleteRepositoryAsync(Repository repository)
    {
        _repositories.Remove(repository);
        repository.Dispose();
        await _repositoryStore.DeleteAsync(repository);
    }

    private static async Task<Repository?> CreateRepositoryOrSetErrorAsync(RepositoryConfigViewModel configViewModel)
    {
        Repository repository = configViewModel.ToRepository();

        try
        {
            await repository.TestConnectionOrThrowAsync();
            return repository;
        }
        catch (Exception ex)
        {
            configViewModel.ErrorMessage = ex.Message;
            repository.Dispose();
            return null;
        }
    }
}
