using DaFiles.Models;
using System.Collections.Generic;

namespace DaFiles.Services;

public interface IPlatformStorage
{
    void MoveItemsToTrash(IEnumerable<DirectoryItem> items, Directory parentDirectory);
}
