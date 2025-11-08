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
        RepositoryConnectViewModel connectViewModel = new();
        RepositoryConnectView connectView = new()
        {
            DataContext = connectViewModel,
        };
        ContentDialog dialog = new()
        {
            PrimaryButtonText = "Connect",
            CloseButtonText = "Cancel",
            Title = "Connect to a remote directory",
            Content = connectView,
        };

        dialog.PrimaryButtonClick += async (dialog, e) =>
        {
            connectViewModel.ErrorMessage = null;

            if (ViewModel is not MainViewModel mainViewModel)
                return;

            connectView.IsEnabled = false;
            var d = e.GetDeferral();

            (dialog.Content as RepositoryConnectView)?.ReadPasswordSecureToViewModel();

            if (!await mainViewModel.TryAddNewRepositoryAsync(connectViewModel, select: true))
                e.Cancel = true;

            d.Complete();
            connectView.IsEnabled = true;
        };

        await dialog.ShowAsync(TopLevel.GetTopLevel(this));
    }
}
