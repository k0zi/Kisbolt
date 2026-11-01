using Microsoft.EntityFrameworkCore;

namespace Kisbolt.Core.Data;

/// <summary>Creates a short-lived <see cref="KisboltDbContext"/> per unit of work.</summary>
public sealed class KisboltDbFactory(DbContextOptions<KisboltDbContext> options) : IDbContextFactory<KisboltDbContext>
{
    public KisboltDbContext CreateDbContext() => new(options);

    public static KisboltDbFactory ForFile(string databasePath) => new(KisboltDbContext.CreateOptions(databasePath));
}
