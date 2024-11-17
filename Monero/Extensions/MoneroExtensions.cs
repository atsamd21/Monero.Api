using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monero.Daemon;
using Monero.Initialization;
using Monero.Models;
using Monero.Wallet;
using Microsoft.AspNetCore.Builder;
using Monero.Singletons;

namespace Monero.Extensions;

public static class MoneroExtensions
{
    public static IServiceCollection AddMonero(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.Configure<MoneroSettings>(configuration.GetSection("MoneroSettings"));
        services.AddScoped<IMoneroWalletService, MoneroWalletService>();
        services.AddScoped<IMoneroDaemonService, MoneroDaemonService>();

        services.AddHttpClient<IMoneroWalletService, MoneroWalletService>();
        services.AddHttpClient<IMoneroDaemonService, MoneroDaemonService>();

        services.AddActivatedSingleton<MoneroPriceSingleton>();

        return services;
    }

    public static IApplicationBuilder UseMonero(this IApplicationBuilder app)
    {
        app.InitializeMonero();

        return app;
    }
}
