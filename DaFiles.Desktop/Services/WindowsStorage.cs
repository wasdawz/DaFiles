using DaFiles.Models;
using DaFiles.Services;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;

namespace DaFiles.Desktop.Services;

[SupportedOSPlatform("windows6.0.6000")]
internal class WindowsStorage : IPlatformStorage
{
    public void MoveItemsToTrash(IEnumerable<DirectoryItem> items, Directory parentDirectory)
    {
        PInvoke.CoCreateInstance(typeof(FileOperation).GUID, null, CLSCTX.CLSCTX_ALL, out IFileOperation fileOperation)
            .ThrowOnFailure();

        fileOperation.SetOperationFlags(FILEOPERATION_FLAGS.FOFX_RECYCLEONDELETE);

        Guid shellItemGuid = typeof(IShellItem).GUID;

        foreach (DirectoryItem item in items)
        {
            string itemPath = parentDirectory.Repository.CombinePath(parentDirectory.Path, item.Name);

            PInvoke.SHCreateItemFromParsingName(itemPath, null, shellItemGuid, out object ppv)
                .ThrowOnFailure();

            if (ppv is not IShellItem shellItem)
                continue;

            fileOperation.DeleteItem(shellItem, null);
        }

        fileOperation.PerformOperations();
    }
}
