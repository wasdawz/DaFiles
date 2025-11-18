using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaFiles.Helpers;
using DaFiles.Services.Repositories;

namespace DaFiles.Models;

public partial class Directory(string path, IRepository repository, List<DirectoryItem> items) : ObservableObject
{
    private string _path = path;
    public string Path
    {
        get => _path;
        private set => SetProperty(ref _path, value);
    }

    public RangeObservableCollection<DirectoryItem> Items { get; } = new(items);

    public IRepository Repository { get; } = repository;

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var items = await Repository.ReadDirectoryContentsAsync(Path);
        Items.ReplaceRange(items);
    }
}
