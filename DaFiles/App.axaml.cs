using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using System;
using DaFiles.Services.Repositories;
using DaFiles.ViewModels;
using DaFiles.Views;

namespace DaFiles;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        LocalRepository localRepository = new();
        MainViewModel mainViewModel = new()
        {
            CurrentRepositoryView = new(new("This device", localRepository))
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow window = new()
            {
                DataContext = mainViewModel
            };

            desktop.MainWindow = window;
            localRepository.TopLevelGetter = () => desktop.MainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            MainView view = new()
            {
                DataContext = mainViewModel
            };

            singleViewPlatform.MainView = view;
            localRepository.TopLevelGetter = () => TopLevel.GetTopLevel(singleViewPlatform.MainView);
        }

        await mainViewModel.LoadAsync();

        base.OnFrameworkInitializationCompleted();
    }
}
