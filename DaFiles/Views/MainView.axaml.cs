using Avalonia.Controls;
using DaFiles.ViewModels;
using FluentAvalonia.UI.Controls;

namespace DaFiles.Views;

public partial class MainView : UserControl
{
    public MainViewModel? ViewModel => DataContext as MainViewModel;

    public MainView()
    {
        InitializeComponent();
    }

    private void PathTextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is not MainViewModel viewModel ||
            sender is not TextBox textBox)
            return;

        textBox.Text = viewModel?.CurrentRepositoryView?.GetCurrentDirectory()?.Directory.Path;
    }

    private async void AddRemoteDirectoryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        RepositoryConfigViewModel configViewModel = new();
        RepositoryConfigView configView = new()
        {
            DataContext = configViewModel,
        };
        ContentDialog dialog = new()
        {
            PrimaryButtonText = "Connect",
            CloseButtonText = "Cancel",
            Title = "Connect to a remote directory",
            Content = configView,
        };

        dialog.PrimaryButtonClick += async (dialog, e) =>
        {
            configViewModel.ErrorMessage = null;

            if (ViewModel is not MainViewModel mainViewModel)
                return;

            configView.IsEnabled = false;
            var d = e.GetDeferral();

            (dialog.Content as RepositoryConfigView)?.ReadPasswordSecureToViewModel();

            if (!await mainViewModel.TryAddNewRepositoryAsync(configViewModel, select: true))
                e.Cancel = true;

            d.Complete();
            configView.IsEnabled = true;
        };

        await dialog.ShowAsync(TopLevel.GetTopLevel(this));
    }

    private async void ConfigureRemoteDirectoryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is not MainViewModel mainViewModel ||
            mainViewModel.CurrentRepositoryView is not RepositoryViewModel currentRepositoryView)
            return;

        RepositoryConfigViewModel configViewModel = new(currentRepositoryView.Repository.Config, preserveId: true);
        RepositoryConfigView configView = new()
        {
            DataContext = configViewModel,
        };
        ContentDialog dialog = new()
        {
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            Title = "Configure remote directory",
            Content = configView,
        };

        dialog.PrimaryButtonClick += async (dialog, e) =>
        {
            configViewModel.ErrorMessage = null;
            configView.IsEnabled = false;
            var d = e.GetDeferral();

            (dialog.Content as RepositoryConfigView)?.ReadPasswordSecureToViewModel();

            if (!await mainViewModel.TryUpdateRepositoryConfigAsync(currentRepositoryView, configViewModel))
                e.Cancel = true;

            d.Complete();
            configView.IsEnabled = true;
        };

        await dialog.ShowAsync(TopLevel.GetTopLevel(this));
    }

    private async void DeleteRemoteDirectoryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is MainViewModel mainViewModel &&
            mainViewModel.CurrentRepositoryView is RepositoryViewModel currentRepositoryView)
        {
            await mainViewModel.DeleteRepositoryAsync(currentRepositoryView);
        }
    }
}
