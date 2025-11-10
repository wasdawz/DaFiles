using Avalonia.Controls;
using Avalonia.Data;
using DaFiles.ViewModels;

namespace DaFiles.Views;

public partial class RepositoryConfigView : UserControl
{
    public RepositoryConfigViewModel? ViewModel => DataContext as RepositoryConfigViewModel;

    public RepositoryConfigView()
    {
        InitializeComponent();
    }

    public void ReadPasswordSecureToViewModel()
    {
        BindingOperations.GetBindingExpressionBase(PasswordTextBox, TextBox.TextProperty)?.UpdateSource();
    }
}
