using Avalonia.Controls;
using DaFiles.ViewModels;
using System.Security;

namespace DaFiles.Views;

public partial class RepositoryConnectView : UserControl
{
    public RepositoryConnectViewModel? ViewModel => DataContext as RepositoryConnectViewModel;

    public RepositoryConnectView()
    {
        InitializeComponent();
    }

    public void ReadPasswordSecureToViewModel()
    {
        if (ViewModel is not RepositoryConnectViewModel viewModel ||
            PasswordTextBox.Text is not string password)
            return;

        SecureString securePassword = new();
        foreach (char ch in password)
            securePassword.AppendChar(ch);
        viewModel.Password = securePassword;
    }
}
