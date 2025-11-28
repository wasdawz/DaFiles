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

    private async void PointerReleasedHandler(object? sender, PointerReleasedEventArgs e)
    {
        if (ViewModel is null)
            return;

        if (e.Properties.PointerUpdateKind == PointerUpdateKind.XButton1Released)
        {
            e.Handled = true;
            await ViewModel.DirectoryNavigation.GoBackAsync();
        }
        else if (e.Properties.PointerUpdateKind == PointerUpdateKind.XButton2Released)
        {
            e.Handled = true;
            await ViewModel.DirectoryNavigation.GoForwardAsync();
        }
    }
}
