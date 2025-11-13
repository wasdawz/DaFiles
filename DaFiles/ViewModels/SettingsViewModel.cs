namespace DaFiles.ViewModels;

public class SettingsViewModel(RepositoriesSettingsViewModel repositoriesSettings) : ViewModelBase
{
    public RepositoriesSettingsViewModel RepositoriesSettings { get; } = repositoriesSettings;
}
