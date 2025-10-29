using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace DaFiles.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private RepositoryViewModel? currentRepositoryView;

    public async Task LoadAsync()
    {
        if (CurrentRepositoryView is null)
            return;

        await CurrentRepositoryView.LoadAsync();
    }
}
