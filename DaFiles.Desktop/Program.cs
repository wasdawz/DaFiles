using System;

using Avalonia;
using DaFiles.Desktop.Services;
using DaFiles.Services;

namespace DaFiles.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        ISecretStore secretStore;
        IPlatformStorage platformStorage;

        if (OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
            secretStore = new WindowsCredentialManagerSecretStore(App.AppName);
        else
            throw new PlatformNotSupportedException();

        if (OperatingSystem.IsWindowsVersionAtLeast(6, 0, 6000))
            platformStorage = new WindowsStorage();
        else
            throw new PlatformNotSupportedException();

        return AppBuilder.Configure<App>(
            () => new()
            {
                SecretStore = secretStore,
                PlatformStorage = platformStorage,
            })
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }

}
