using System.Text.Json;
using Kisbolt.Core.Forges;

namespace Kisbolt.Core.Metadata;

/// <summary>
/// Store metadata a repository can publish in <c>.kisbolt/kisbolt.json</c>:
/// <code>{ "name": "MyApp", "version": "1.2.3", "author": "Me", "icon": "icon.png" }</code>
/// <c>icon</c> is a path relative to the <c>.kisbolt</c> folder.
/// </summary>
public sealed record KisboltManifest(string? Name, string? Version, string? Author, string? Icon)
{
    public const string Folder = ".kisbolt";
    public const string FileName = "kisbolt.json";

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>Parses the manifest; returns <c>null</c> for malformed content.</summary>
    public static KisboltManifest? Parse(ReadOnlySpan<byte> json)
    {
        try
        {
            var manifest = JsonSerializer.Deserialize<KisboltManifest>(json, Options);
            return manifest is null ? null : manifest with
            {
                Name = Clean(manifest.Name),
                Version = Clean(manifest.Version),
                Author = Clean(manifest.Author),
                Icon = Clean(manifest.Icon),
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>The icon path inside the repository, or <c>null</c> if the manifest names none or escapes the folder.</summary>
    public string? IconRepoPath
    {
        get
        {
            if (Icon is null)
                return null;
            var relative = Icon.Replace('\\', '/').TrimStart('/');
            if (relative.StartsWith("./", StringComparison.Ordinal))
                relative = relative[2..];
            return relative.Split('/').Any(s => s is ".." or "") ? null : $"{Folder}/{relative}";
        }
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

/// <summary>A downloaded <see cref="KisboltManifest"/> and its icon.</summary>
public sealed record KisboltMetadata(KisboltManifest Manifest, byte[]? Icon, string? IconExtension);

public sealed class KisboltManifestReader
{
    private static readonly string[] IconExtensions = [".png", ".jpg", ".jpeg", ".webp", ".ico", ".bmp"];

    /// <summary>Reads <c>.kisbolt/kisbolt.json</c> and its icon; <c>null</c> when the repository has none.</summary>
    public async Task<KisboltMetadata?> ReadAsync(IForgeClient client, RepoRef repo, CancellationToken ct = default)
    {
        var json = await client.TryGetFileAsync(repo, $"{KisboltManifest.Folder}/{KisboltManifest.FileName}", ct)
                               .ConfigureAwait(false);
        if (json is null || KisboltManifest.Parse(json) is not { } manifest)
            return null;

        var iconPath = manifest.IconRepoPath;
        var extension = iconPath is null ? null : Path.GetExtension(iconPath).ToLowerInvariant();
        if (iconPath is null || !IconExtensions.Contains(extension))
            return new KisboltMetadata(manifest, null, null);

        var icon = await client.TryGetFileAsync(repo, iconPath, ct).ConfigureAwait(false);
        return new KisboltMetadata(manifest, icon, icon is null ? null : extension);
    }
}
