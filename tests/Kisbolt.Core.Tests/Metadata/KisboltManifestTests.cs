using System.Text;
using Kisbolt.Core.Metadata;

namespace Kisbolt.Core.Tests.Metadata;

public class KisboltManifestTests
{
    private static KisboltManifest? Parse(string json) => KisboltManifest.Parse(Encoding.UTF8.GetBytes(json));

    [Fact]
    public void Parses_all_fields()
    {
        var manifest = Parse("""
            { "name": "My App", "version": "1.2.3", "author": " Me ", "icon": "icon.png", // comment
            }
            """);
        Assert.Equal(new KisboltManifest("My App", "1.2.3", "Me", "icon.png"), manifest);
        Assert.Equal(".kisbolt/icon.png", manifest!.IconRepoPath);
    }

    [Fact]
    public void Empty_values_become_null() =>
        Assert.Equal(new KisboltManifest(null, null, null, null), Parse("""{ "name": "  " }"""));

    [Fact]
    public void Malformed_json_is_null() => Assert.Null(Parse("not json"));

    [Theory]
    [InlineData("./img/logo.png", ".kisbolt/img/logo.png")]
    [InlineData("/logo.png", ".kisbolt/logo.png")]
    [InlineData("../secret.png", null)]
    [InlineData("a/../../b.png", null)]
    public void Icon_path_stays_inside_folder(string icon, string? expected) =>
        Assert.Equal(expected, new KisboltManifest(null, null, null, icon).IconRepoPath);
}
