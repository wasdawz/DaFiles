using DaFiles.Models;
using DaFiles.Services;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public class RepositoriesSettingsViewModel : ViewModelBase
{
    public ReadOnlyObservableCollection<Repository> Repositories { get; }

    private readonly SourceList<Repository> _repositories;
    private readonly IDataStore<Repository> _repositoryStore;

    public RepositoriesSettingsViewModel(SourceList<Repository> repositories, IDataStore<Repository> repositoryStore)
    {
        _repositories = repositories;
        _repositoryStore = repositoryStore;
        _repositories.Connect().Filter(r => r.Config.IsSaveable).Bind(out var editableRepositories).Subscribe();
        Repositories = editableRepositories;
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
        Repository? repository = null;

        try
        {
            repository = configViewModel.ToRepository();
            await repository.TestConnectionOrThrowAsync();
            return repository;
        }
        catch (Exception ex)
        {
            configViewModel.ErrorMessage = ex.Message;
            repository?.Dispose();
            return null;
        }
    }
}
