using CommunityToolkit.Mvvm.ComponentModel;
using DaFiles.Models;
using DaFiles.Services.Repositories;
using System;
using System.Threading.Tasks;

namespace DaFiles.Services;

public partial class OperationManager : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StagedPasteableOperation))]
    private Operation? stagedOperation;

    public TransferOperation? StagedPasteableOperation => StagedOperation as TransferOperation;

    public async Task RunOperationAsync(Operation operation)
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
        else
            throw new NotImplementedException();
    }

    private static async Task RunTransferOperationAsync(TransferOperation operation)
    {
        if (operation.Destination is null)
            return;

        if (operation.Source.Repository is LocalRepository localSourceRepository &&
            operation.Destination.Repository is LocalRepository)
        {
            if (operation.OperationType == TransferOperationType.Cut)
                await localSourceRepository.MoveItemsAsync(operation.Items, operation.Destination);
            else if (operation.OperationType == TransferOperationType.Copy)
                await localSourceRepository.CopyItemsAsync(operation.Items, operation.Destination);
            else
                throw new NotImplementedException();
        }
        else
            throw new NotImplementedException();
    }
}
