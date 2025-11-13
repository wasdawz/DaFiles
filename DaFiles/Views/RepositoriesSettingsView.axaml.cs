using Avalonia.Controls;
using DaFiles.Models;
using DaFiles.ViewModels;
using FluentAvalonia.UI.Controls;

namespace DaFiles.Views;

public partial class RepositoriesSettingsView : UserControl
{
    public RepositoriesSettingsViewModel? ViewModel => DataContext as RepositoriesSettingsViewModel;

    public RepositoriesSettingsView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
            DataContext = Dummy.CreateDummyRepositoriesSettingsViewModel();
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

            if (ViewModel is not RepositoriesSettingsViewModel viewModel)
                return;

            configView.IsEnabled = false;
            var d = e.GetDeferral();

            (dialog.Content as RepositoryConfigView)?.ReadPasswordSecureToViewModel();

            if (!await viewModel.TryAddNewRepositoryAsync(configViewModel))
                e.Cancel = true;

            d.Complete();
            configView.IsEnabled = true;
        };

        await dialog.ShowAsync(TopLevel.GetTopLevel(this));
    }

    private async void ConfigureRemoteDirectoryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is not RepositoriesSettingsViewModel viewModel ||
            (sender as Control)?.DataContext is not Repository repository)
            return;

        RepositoryConfigViewModel configViewModel = new(repository.Config, preserveId: true);
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

            if (!await viewModel.TryUpdateRepositoryConfigAsync(repository, configViewModel))
                e.Cancel = true;

            d.Complete();
            configView.IsEnabled = true;
        };

        await dialog.ShowAsync(TopLevel.GetTopLevel(this));
    }

    private async void DeleteRemoteDirectoryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is RepositoriesSettingsViewModel viewModel &&
            sender is Control { DataContext: Repository repository })
        {
            await viewModel.DeleteRepositoryAsync(repository);
        }
    }
}
