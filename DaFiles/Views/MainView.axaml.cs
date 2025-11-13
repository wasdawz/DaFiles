using Avalonia.Controls;
using DaFiles.Helpers;
using DaFiles.ViewModels;

namespace DaFiles.Views;

public partial class MainView : UserControl
{
    public MainViewModel? ViewModel => DataContext as MainViewModel;

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
}
