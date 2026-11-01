using Kisbolt.App.Services;
using Kisbolt.App.ViewModels;
using Kisbolt.Core;
using Kisbolt.Core.Data;
using Kisbolt.Core.Forges;
using Kisbolt.Core.Metadata;
using Kisbolt.Core.Platform;
using Kisbolt.Core.Services;
using KD.Avalonia.Rice.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kisbolt.App;

internal static class CompositionRoot
{
    public static ServiceProvider Build(RiceWindow mainWindow)
    {
        var paths = new AppPaths();
        paths.EnsureCreated();
        var dbFactory = KisboltDbFactory.ForFile(paths.DatabasePath);
        using (var db = dbFactory.CreateDbContext())
            db.Database.Migrate();

        // The default timeout ends when the response headers arrive; downloads stream after that.
        var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd($"Kisbolt/{typeof(App).Assembly.GetName().Version?.ToString(3)}");

        var platform = PlatformDetector.Detect();

        var services = new ServiceCollection();
        services.AddSingleton(paths);
        services.AddSingleton(platform);
        services.AddSingleton(http);
        services.AddSingleton<IDbContextFactory<KisboltDbContext>>(dbFactory);
        services.AddSingleton<CredentialService>();
        services.AddSingleton<ITokenProvider>(sp => sp.GetRequiredService<CredentialService>());
        services.AddSingleton<IForgeClient, GitHubClient>();
        services.AddSingleton<IForgeClient, GitLabClient>();
        services.AddSingleton<IForgeClient, ForgejoClient>();
        services.AddSingleton<ForgeResolver>();
        services.AddSingleton<KisboltManifestReader>();
        services.AddSingleton<AssetDownloader>();
        services.AddSingleton<IProcessRunner, ProcessRunner>();
        services.AddSingleton(sp => CreateInstaller(platform, sp.GetRequiredService<IProcessRunner>()));
        services.AddSingleton<StoreService>();

        services.AddSingleton<IDialogService>(new DialogService(mainWindow));
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<StoreViewModel>();
        services.AddSingleton<SettingsViewModel>();
        return services.BuildServiceProvider();
    }

    private static IPackageInstaller CreateInstaller(PlatformInfo platform, IProcessRunner runner) =>
        platform.PackageManager switch
        {
            PackageManager.Windows when OperatingSystem.IsWindows() => new WindowsInstaller(),
            PackageManager.Apt or PackageManager.Dnf or PackageManager.Zypper =>
                new LinuxInstaller(platform.PackageManager, runner),
            _ => new UnsupportedInstaller(),
        };
}
