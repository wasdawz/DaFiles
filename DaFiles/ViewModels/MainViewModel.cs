using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaFiles.Models;
using DaFiles.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private RepositoryViewModel? currentRepositoryView;

    public ReadOnlyObservableCollection<RepositoryViewModel> Repositories { get; }

    private readonly ObservableCollection<RepositoryViewModel> _repositories;
    private readonly IDataStore<Repository> _repositoryStore;

    public MainViewModel(IEnumerable<Repository> repositories, IDataStore<Repository> repositoryStore)
    {
        _repositories = new(repositories.Select(r => new RepositoryViewModel(r)));
        Repositories = new(_repositories);
        _repositoryStore = repositoryStore;
    }

    public async Task LoadAsync()
    {
        CurrentRepositoryView ??= Repositories.FirstOrDefault();

        if (CurrentRepositoryView is null)
            return;

        await CurrentRepositoryView.EnsureInitializedAsync();
    }

    [RelayCommand]
    public async Task SelectRepositoryAsync(RepositoryViewModel repository)
    {
        CurrentRepositoryView = repository;
        await repository.EnsureInitializedAsync();
    }

    public async Task<bool> TryAddNewRepositoryAsync(RepositoryConnectViewModel connectViewModel, bool select = false)
    {
        Repository repository = connectViewModel.ToRepository();

        try
        {
            await repository.TestConnectionOrThrowAsync();
        }
        catch (Exception ex)
        {
            connectViewModel.ErrorMessage = ex.Message;
            return false;
        }

        RepositoryViewModel repositoryViewModel = new(repository);
        _repositories.Add(repositoryViewModel);

        if (select)
            await SelectRepositoryAsync(repositoryViewModel);

        await _repositoryStore.SaveAsync(Repositories.Select(vm => vm.Repository));
        return true;
    }
}
