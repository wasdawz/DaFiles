using CommunityToolkit.Mvvm.ComponentModel;
using DaFiles.Helpers;
using DaFiles.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DaFiles.ViewModels;

public partial class DirectoryViewModel(Directory directory) : ViewModelBase, INavigationItem<string>
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
}
