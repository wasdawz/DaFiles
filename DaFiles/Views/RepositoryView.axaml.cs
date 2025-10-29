using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Threading.Tasks;
using DaFiles.ViewModels;

namespace DaFiles.Views;

public partial class RepositoryView : UserControl
{
    public RepositoryViewModel? ViewModel => DataContext as RepositoryViewModel;

    public RepositoryView()
    {
        InitializeComponent();

        AddHandler(DoubleTappedEvent, DoubleTappedHandler);
        AddHandler(PointerReleasedEvent, PointerReleasedHandler);
    }

    private async Task DoubleTappedHandler(object? sender, TappedEventArgs e)
    {
        if (e.Source is Visual source && source.DataContext is not null && ViewModel is RepositoryViewModel viewModel)
        {
            await viewModel.OpenItemAsync(source.DataContext);
            e.Handled = true;
        }
    }

    private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Properties.PointerUpdateKind == PointerUpdateKind.XButton1Released)
            ViewModel?.DirectoryNavigation.GoBack();
        else if (e.Properties.PointerUpdateKind == PointerUpdateKind.XButton2Released)
            ViewModel?.DirectoryNavigation.GoForward();
        else
            return;

        e.Handled = true;
    }

    private void PathTextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is not RepositoryViewModel viewModel ||
            sender is not TextBox textBox)
            return;

        textBox.Text = viewModel?.GetCurrentDirectory()?.Directory.Path;
    }
}
