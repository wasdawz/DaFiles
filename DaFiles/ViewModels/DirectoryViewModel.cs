using CommunityToolkit.Mvvm.ComponentModel;
using DaFiles.Helpers;
using DaFiles.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public partial class DirectoryViewModel(Directory directory) : ViewModelBase, INavigationItem<string>, IRefreshAsync
{
    public Directory Directory { get; } = directory;

    [ObservableProperty]
    private DirectoryItem? selectedItem;

    public IList? SelectedItems { set; private get; }

    string INavigationItem<string>.NavigationKey => Directory.Path;

    public IList<DirectoryItem>? GetSelectedItems()
    {
        return SelectedItems?.OfType<DirectoryItem>().ToList();
    }

    async Task IRefreshAsync.RefreshAsync()
    {
        await Directory.RefreshAsync();
    }
}
