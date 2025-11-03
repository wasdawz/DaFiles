using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaFiles.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private RepositoryViewModel? currentRepositoryView;

    public ReadOnlyObservableCollection<RepositoryViewModel> Repositories { get; }

    private readonly ObservableCollection<RepositoryViewModel> _repositories = [];

    public MainViewModel()
    {
        Repositories = new(_repositories);
    }

    public async Task LoadAsync()
    {
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

    public async Task<bool> TryAddRepositoryAsync(RepositoryConnectViewModel connectViewModel, bool select = false)
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
        AddRepository(repositoryViewModel);
        await SelectRepositoryAsync(repositoryViewModel);
        return true;
    }

    public void AddRepository(Repository repository)
    {
        RepositoryViewModel repositoryViewModel = new(repository);
        AddRepository(repositoryViewModel);
    }

    private void AddRepository(RepositoryViewModel repositoryViewModel)
    {
        _repositories.Add(repositoryViewModel);
        CurrentRepositoryView ??= repositoryViewModel;
    }
}
