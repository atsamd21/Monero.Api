using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Monero.Daemon;
using Monero.Models;
using Monero.Wallet;

namespace Monero.Initialization;

public static class Initializer
{
    public static IApplicationBuilder InitializeMonero(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var moneroWalletService = scope.ServiceProvider.GetRequiredService<IMoneroWalletService>();

        var settings = scope.ServiceProvider.GetRequiredService<IOptions<MoneroSettings>>().Value;

        if (!settings.UseRemoteNode)
        {
            var moneroDaemonService = scope.ServiceProvider.GetRequiredService<IMoneroDaemonService>();

            var daemonStartedSuccessfully = moneroDaemonService.StartDaemon();
            if (!daemonStartedSuccessfully)
            {
                throw new Exception("Could not start monero daemon.");
            }
        }

        var moneroWalletStartedSuccessfully = moneroWalletService.StartWalletRPC();
        if (!moneroWalletStartedSuccessfully)
        {
            throw new Exception("Could not start monero wallet rpc.");
        }

        if (!moneroWalletService.CheckIfWalletExists("master"))
        {
            var createSuccessful = moneroWalletService.CreateWalletAsync("master").GetAwaiter().GetResult();
            if (!createSuccessful)
            {
                throw new Exception("Wallet does not exist and it could not be created.");
            }
        }
        else
        {
            var openSuccessful = moneroWalletService.OpenWalletAsync("master").GetAwaiter().GetResult();
            if (!openSuccessful)
            {
                throw new Exception("Could not open wallet.");
            }

            moneroWalletService.RefreshWalletAsync().GetAwaiter().GetResult();
        }

        Console.WriteLine("Finished initializing monero services.");

        return app;
    }
}
