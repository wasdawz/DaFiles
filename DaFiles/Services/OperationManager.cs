using CommunityToolkit.Mvvm.ComponentModel;
using DaFiles.Helpers;
using DaFiles.Models;
using DaFiles.Services.Repositories;
using System;
using System.Threading.Tasks;

namespace DaFiles.Services;

public partial class OperationManager(IMessagePresenter messagePresenter) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StagedPasteableOperation))]
    private Operation? stagedOperation;

    public TransferOperation? StagedPasteableOperation => StagedOperation as TransferOperation;

    private readonly IMessagePresenter _messagePresenter = messagePresenter;

    public async Task RunOperationAsync(Operation operation)
    {
        try
        {
            if (operation is TransferOperation transferOperation)
            {
                if (transferOperation.Destination is null)
                    return;

                if (transferOperation == StagedOperation &&
                    transferOperation.OperationType == TransferOperationType.Cut)
                {
                    StagedOperation = null;
                }

                await Task.Run(() => RunTransferOperationAsync(transferOperation));
                await transferOperation.Destination.RefreshAsync();
                await transferOperation.Source.RefreshAsync();
            }
            else if (operation is DeleteOperation deleteOperation)
            {
                await Task.Run(() => RunDeleteOperationAsync(deleteOperation));
                await deleteOperation.ParentDirectory.RefreshAsync();
            }
            else
                throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            _messagePresenter.ShowError(ex.Message);
        }
    }

    private static async Task RunTransferOperationAsync(TransferOperation operation)
    {
        var items = operation.Items;
        var source = operation.Source;
        var destination = operation.Destination;
        var operationType = operation.OperationType;

        if (destination is null)
            return;

        bool transferredWithinPlatform;

        if (operationType == TransferOperationType.Cut)
            transferredWithinPlatform = await source.Repository.MoveItemsWithinPlatformAsync(items, source, destination);
        else if (operationType == TransferOperationType.Copy)
            transferredWithinPlatform = await source.Repository.CopyItemsWithinPlatformAsync(items, source, destination);
        else
            throw new NotImplementedException();

        if (transferredWithinPlatform)
            return;

        if (operationType == TransferOperationType.Copy)
            await destination.Repository.WriteItemsAsync(items, source, destination);
        else
            throw new NotImplementedException();
    }

    private static async Task RunDeleteOperationAsync(DeleteOperation operation)
    {
        if (operation.ParentDirectory.Repository is LocalRepository localRepository)
        {
            if (operation.Permanent)
                await LocalRepository.DeleteItemsAsync(operation.Items);
            else
                localRepository.MoveItemsToTrash(operation.Items, operation.ParentDirectory);
        }
        else
            throw new NotImplementedException();
    }
}
