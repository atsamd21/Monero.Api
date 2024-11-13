using Microsoft.EntityFrameworkCore;

namespace Monero.Api.Data;

public static class DatabaseBuilderExtensions
{
    private static async Task Seed()
    {
        await Task.Delay(15);
    }

    public static IApplicationBuilder MigrateAndSeedDb<TDbContext>(this IApplicationBuilder app) where TDbContext : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<TDbContext>();

        db.Database.Migrate();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        Seed().GetAwaiter().GetResult();

        return app;
    }
}