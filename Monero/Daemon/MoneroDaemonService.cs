using System.Diagnostics;
using Microsoft.Extensions.Options;
using System.Net;
using System.Runtime.InteropServices;
using Monero.Models;
using Monero.Shared;
using Monero.Daemon.Responses;

namespace Monero.Daemon;

public interface IMoneroDaemonService
{
    Task<FeeEstimateResponse> GetFeeEstimateAsync();
    bool StartDaemon();
}

public sealed class MoneroDaemonService : IMoneroDaemonService
{
    private readonly MoneroRPCClient _moneroRPCClient;
    private readonly MoneroSettings _settings;

    public MoneroDaemonService(HttpClient httpClient, IOptions<MoneroSettings> options)
    {
        var port = options.Value.Network switch
        {
            Network.Mainnet => 18081,
            Network.Testnet => 28081,
            Network.Stagenet => 38081,
            _ => throw new ArgumentException("Not a valid network."),
        };

        string daemonAddress;

        if (options.Value.UseRemoteNode)
        {
            if (!IPAddress.TryParse(options.Value.DaemonAddress, out var _))
                throw new ArgumentException("DaemonAddress is not a valid address.");

            daemonAddress = options.Value.DaemonAddress;
        }
        else
        {
            daemonAddress = "127.0.0.1";
        }

        _settings = options.Value;
        _moneroRPCClient = new MoneroRPCClient(httpClient, daemonAddress, port);
    }

    public async Task<FeeEstimateResponse> GetFeeEstimateAsync()
    {
        var response = await _moneroRPCClient.GetAsync<FeeEstimateResponse>(new BaseRequest("get_fee_estimate"));

        if (response is null || response.Result is null)
            return default!;

        return response.Result;
    }

    public bool StartDaemon()
    {
        try
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(currentDirectory))
                return false;

            // Kill monerod if already running
            var procesess = Process.GetProcesses();
            foreach (var p in procesess)
            {
                var processName = p.ProcessName.ToLower();

                if (processName.CompareTo("monerod") == 0)
                {
                    p.Kill();
                    break;
                }
            }

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            ProcessStartInfo startInfo = new()
            {
                FileName = isWindows ? Path.Combine(currentDirectory, "Programs", "monerod.exe") : Path.Combine(currentDirectory, "Programs", "monerod"),
                Arguments = $"{(_settings.Network != Network.Mainnet ? $"--{_settings.Network}" : "")} {(string.IsNullOrEmpty(_settings.DataDirectory) ? "" : $"--data-dir={_settings.DataDirectory}")} --rpc-bind-port={_moneroRPCClient.Port} --no-zmq --out-peers=32 --hide-my-port --no-igd",
                RedirectStandardOutput = true,
                WorkingDirectory = currentDirectory
            };

            var process = Process.Start(startInfo);

            if (process is null)
                return false;

            process.Exited += (sender, e) =>
            {
                Console.WriteLine("MONEROD EXITED");
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                process.Kill();
                process.Dispose();
            };

            using var reader = process.StandardOutput;
            while (true)
            {
                var line = reader.ReadLine();
                Console.WriteLine(line);

                if (line is null)
                    throw new Exception("Line was null in MoneroDaemonClient.");

                if (line.Contains("You are now synchronized with the network."))
                    return true;
            }
        }
        catch
        {
            return false;
        }
    }
}
