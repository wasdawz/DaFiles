using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using DaFiles.Data;
using DaFiles.Models;
using DaFiles.Services;
using DaFiles.ViewModels;
using DaFiles.Views;
using System;

namespace DaFiles;

public partial class App(ISecretStore secretStore) : Application
{
    public const string AppName = "DaFiles";

    private readonly ISecretStore _secretStore = secretStore;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        Control rootControl;
        Func<TopLevel?>? topLevelGetter;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow window = new();
            desktop.MainWindow = window;
            rootControl = window;
            topLevelGetter = () => desktop.MainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            MainView view = new();
            singleViewPlatform.MainView = view;
            rootControl = view;
            topLevelGetter = () => TopLevel.GetTopLevel(singleViewPlatform.MainView);
        }
        else if (ApplicationLifetime is null && Design.IsDesignMode)
        {
            return;
        }
        else
            throw new NotImplementedException();

        Repositories repositoryStore = new(PrepareDataDirectory("Repositories"), topLevelGetter, _secretStore);
        var repositories = await repositoryStore.ReadAllAsync() ?? [];
        repositories.Insert(0, new Repository(new LocalRepositoryConfig("This device", topLevelGetter)));

        MainViewModel mainViewModel = new(repositories, repositoryStore);

        rootControl.DataContext = mainViewModel;

        await mainViewModel.LoadAsync();

        base.OnFrameworkInitializationCompleted();
    }

    private static string PrepareDataDirectory(string subdirectory)
    {
        string dataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, subdirectory);
        System.IO.Directory.CreateDirectory(dataPath);
        return dataPath;
    }
}
