using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using DaFiles.Services.Repositories;

namespace DaFiles.Models;

public partial class Repository(string name, IRepository service) : ObservableObject
{
    [ObservableProperty]
    private string name = name;

    public IRepository Service { get; } = service;
}
