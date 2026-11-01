namespace Kisbolt.Core;

/// <summary>Locations of the store's local files.</summary>
public sealed class AppPaths
{
    public AppPaths(string? dataDirectory = null)
    {
        DataDirectory = dataDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData,
                                      Environment.SpecialFolderOption.Create),
            "kisbolt");
    }

    /// <summary>Linux: <c>~/.local/share/kisbolt</c>, Windows: <c>%LOCALAPPDATA%\kisbolt</c>.</summary>
    public string DataDirectory { get; }

    public string DatabasePath => Path.Combine(DataDirectory, "kisbolt.db");

    /// <summary>Preferences of the user interface (e.g. the language).</summary>
    public string SettingsPath => Path.Combine(DataDirectory, "settings.json");

    public string IconDirectory => Path.Combine(DataDirectory, "icons");

    public string DownloadDirectory => Path.Combine(Path.GetTempPath(), "kisbolt");

    public void EnsureCreated()
    {
        Directory.CreateDirectory(DataDirectory);
        Directory.CreateDirectory(IconDirectory);
        Directory.CreateDirectory(DownloadDirectory);
    }
}
