using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using DaFiles.Helpers;
using DaFiles.Models;

namespace DaFiles.ViewModels;

public partial class DirectoryViewModel(Directory directory) : ViewModelBase, INavigationItem<string>
{
    public Directory Directory { get; } = directory;

    [ObservableProperty]
    private DirectoryItem? selectedItem;

    string INavigationItem<string>.NavigationKey => Directory.Path;
}
