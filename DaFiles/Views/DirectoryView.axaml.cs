using Avalonia.Controls;
using System;
using DaFiles.Services.Repositories;
using DaFiles.ViewModels;

namespace DaFiles.Views;

public partial class DirectoryView : UserControl
{
    public DirectoryView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            DataContext = new DirectoryViewModel(new("C:/Users/user/Documents", DummyRepository.Instance, [
                new(Models.DirectoryItemType.File, "Library.dll", DateTimeOffset.Parse("2025.10.20 12:29:34"), 850),
                new(Models.DirectoryItemType.File, "Note.txt", DateTimeOffset.Parse("2025.10.20 12:29:34"), 0),
                new(Models.DirectoryItemType.File, "Program.exe", DateTimeOffset.Parse("2025.10.20 12:29:34"), 24899438),
                new(Models.DirectoryItemType.File, "Note 2.txt", DateTimeOffset.Parse("2025.10.20 12:29:34"), 850),
                new(Models.DirectoryItemType.File, "Video.mp4", DateTimeOffset.Parse("2020.11.13 16:29:34"), 34879282892),
                new(Models.DirectoryItemType.Folder, "Folder 1", DateTimeOffset.Parse("2020.11.13 16:29:34"), null),
                new(Models.DirectoryItemType.Folder, "Folder 2", DateTimeOffset.Parse("2022.01.05 08:00:10"), null),
                ]));
        }
    }
}
