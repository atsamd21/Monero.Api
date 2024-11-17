using Microsoft.EntityFrameworkCore;
using Monero.Api.Models;

namespace Monero.Api.Data;

public class MoneroApiContext : DbContext
{
    public DbSet<Payment> Payments { get; set; }

    public string DbPath { get; }

    public MoneroApiContext(DbContextOptions<MoneroApiContext> options) : base(options)
    {
#if DEBUG
        DbPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "moneroapi-dev.db");
#else
        DbPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "moneroapi.db");
#endif
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}").UseLazyLoadingProxies();
}
