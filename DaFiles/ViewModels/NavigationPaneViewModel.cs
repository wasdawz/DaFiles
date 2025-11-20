using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaFiles.Models;
using DaFiles.Services;
using DynamicData;
using System;
using System.Collections.Generic;
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

    public OperationManager OperationManager { get; }

    public NavigationPaneViewModel(SourceList<Repository> repositories, OperationManager operationManager)
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
        OperationManager = operationManager;
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

    [RelayCommand]
    public void CutSelectedItems() => StageSelectedItemsOperation(TransferOperationType.Cut);

    [RelayCommand]
    public void CopySelectedItems() => StageSelectedItemsOperation(TransferOperationType.Copy);

    [RelayCommand]
    public async Task Paste()
    {
        if (OperationManager.StagedPasteableOperation is not TransferOperation pasteOperation)
            return;
        if (CurrentRepositoryView?.DirectoryNavigation.CurrentItem?.Directory is not Directory destinationDirectory)
            return;

        pasteOperation.Destination = destinationDirectory;
        await OperationManager.RunOperationAsync(pasteOperation);
    }

    private void StageSelectedItemsOperation(TransferOperationType operationType)
    {
        if (CurrentRepositoryView?.DirectoryNavigation.CurrentItem is not DirectoryViewModel sourceDirectoryView)
            return;
        if (sourceDirectoryView.GetSelectedItems() is not IList<DirectoryItem> selectedItems)
            return;

        TransferOperation operation = new()
        {
            OperationType = operationType,
            Source = sourceDirectoryView.Directory,
            Items = selectedItems,
        };

        OperationManager.StagedOperation = operation;
    }
}
