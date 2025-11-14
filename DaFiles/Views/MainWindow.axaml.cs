using Avalonia.Controls;
using DaFiles.Helpers;
using DaFiles.ViewModels;
using DynamicData.Binding;
using System;
using System.Reactive.Linq;

namespace DaFiles.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.WhenValueChanged(w => w.DataContext)
            .Select(e => e as MainViewModel)
            .SelectInnerSequenceNullable(vm => vm.WhenValueChanged(vm => vm.IsSecondNavigationPaneOpen))
            .Switch()
            .DistinctUntilChanged()
            .Skip(1)
            .Subscribe(OnIsSecondNavigationPaneOpenChanged);
    }

    private void OnIsSecondNavigationPaneOpenChanged(bool isSecondNavigationPaneOpen)
    {
        if (isSecondNavigationPaneOpen)
            Width *= 2;
        else
            Width /= 2;
    }
}
