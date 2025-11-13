using Avalonia.Controls;
using Avalonia.Interactivity;
using DaFiles.Helpers;
using DaFiles.ViewModels;
using FluentAvalonia.UI.Controls;
using System;
using System.Linq;

namespace DaFiles.Views;

public partial class SettingsView : UserControl
{
    private static readonly INavigationPageFactory NavigationPageFactory = new CustomNavigationPageFactory(e => e switch
    {
        RepositoriesSettingsViewModel vm => new RepositoriesSettingsView() { DataContext = vm },
        _ => throw new NotImplementedException(),
    });

    public SettingsView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
            DataContext = new SettingsViewModel(Dummy.CreateDummyRepositoriesSettingsViewModel());

        ContentFrame.NavigationPageFactory = NavigationPageFactory;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        NavView.SelectedItem = NavView.MenuItems.FirstOrDefault();
    }

    private void NavView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is not NavigationViewItem item)
            return;

        ContentFrame.NavigateFromObject(item.Tag, new()
        {
            IsNavigationStackEnabled = false,
            TransitionInfoOverride = e.RecommendedNavigationTransitionInfo
        });
    }
}
