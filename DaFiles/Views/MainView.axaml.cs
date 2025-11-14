using Avalonia.Controls;
using DaFiles.Helpers;
using DaFiles.ViewModels;

namespace DaFiles.Views;

public partial class MainView : UserControl
{
    public MainViewModel? ViewModel => DataContext as MainViewModel;

    private NavigationPaneView? _lastFocusedNavigationPane;

    public MainView()
    {
        InitializeComponent();
    }

    private void SettingsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SettingsWindow settingsWindow = new()
        {
            DataContext = ViewModel?.Settings
        };
        settingsWindow.ShowTryingAsChild(this);
    }

    private void NavigationPaneView_GotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
    {
        if (sender is not NavigationPaneView focusedPane)
            return;

        if (focusedPane != _lastFocusedNavigationPane && _lastFocusedNavigationPane is not null &&
            _lastFocusedNavigationPane.DataContext is NavigationPaneViewModel lastPaneViewModel)
        {
            lastPaneViewModel.CurrentRepositoryView?.DirectoryNavigation.CurrentItem?.SelectedItem = null;
        }

        _lastFocusedNavigationPane = focusedPane;
    }
}
