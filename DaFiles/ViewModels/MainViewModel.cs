using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaFiles.Models;
using DynamicData;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public partial class MainViewModel(SourceList<Repository> repositories, SettingsViewModel settings) : ViewModelBase
{
    [ObservableProperty]
    private bool isSecondNavigationPaneOpen;

    public NavigationPaneViewModel FirstNavigationPane { get; } = new(repositories);
    public NavigationPaneViewModel SecondNavigationPane { get; } = new(repositories);

    public SettingsViewModel Settings { get; } = settings;

    [RelayCommand]
    public void ToggleSecondPane()
    {
        IsSecondNavigationPaneOpen = !IsSecondNavigationPaneOpen;
    }

    public async Task LoadAsync()
    {
        await FirstNavigationPane.LoadAsync();
        await SecondNavigationPane.LoadAsync();
    }
}
