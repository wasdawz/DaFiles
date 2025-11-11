using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaFiles.Models;
using DynamicData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public partial class NavigationPaneViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCurrentRepositoryEditable))]
    private RepositoryViewModel? currentRepositoryView;

    public ReadOnlyObservableCollection<RepositoryViewModel> RepositoryViews { get; }

    public bool IsCurrentRepositoryEditable => CurrentRepositoryView?.Repository.Config.IsSaveable ?? false;

    public NavigationPaneViewModel(SourceList<Repository> repositories)
    {
        repositories.Connect()
            .Transform(r => new RepositoryViewModel(r))
            .Bind(out var repositoryViews)
            .OnItemRemoved(rvm =>
            {
                if (CurrentRepositoryView == rvm)
                    CurrentRepositoryView = RepositoryViews?.FirstOrDefault();
            })
            .Subscribe();
        RepositoryViews = repositoryViews;
    }

    public async Task LoadAsync()
    {
        CurrentRepositoryView ??= RepositoryViews?.FirstOrDefault();

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
}
