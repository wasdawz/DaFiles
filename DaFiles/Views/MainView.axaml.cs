using Avalonia;
using Avalonia.Controls;
using DaFiles.Helpers;
using DaFiles.ViewModels;
using FluentAvalonia.UI.Controls;

namespace DaFiles.Views;

public partial class MainView : UserControl, IMessagePresenter
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

    async void IMessagePresenter.ShowError(string message)
    {
        TaskDialog dialog = new()
        {
            Title = "Error",
            Header = "Error",
            Content = message,
            MaxWidth = 500,
            XamlRoot = this.VisualRoot as Visual,
            Buttons =
            {
                TaskDialogButton.CancelButton
            }
        };

        await dialog.ShowAsync();
    }
}
