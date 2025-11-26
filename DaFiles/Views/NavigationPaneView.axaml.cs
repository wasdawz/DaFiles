using Avalonia.Controls;
using DaFiles.ViewModels;

namespace DaFiles.Views;

public partial class NavigationPaneView : UserControl
{
    public NavigationPaneViewModel? ViewModel => DataContext as NavigationPaneViewModel;

    public NavigationPaneView()
    {
        InitializeComponent();
    }

    private void PathTextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel is not NavigationPaneViewModel viewModel ||
            sender is not TextBox textBox)
            return;

        textBox.Text = viewModel?.CurrentRepositoryView?.CurrentDirectory?.Directory.Path;
    }
}
