using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kisbolt.App.ViewModels;
using Kisbolt.App.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Kisbolt.App;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new MainWindow();
            var services = CompositionRoot.Build(window);
            desktop.Exit += (_, _) => services.Dispose();
            window.DataContext = services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = window;
        }
        base.OnFrameworkInitializationCompleted();
    }
}
