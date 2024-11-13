using System.Text.Json;
using System.Text;
using System.Net.Http.Json;

namespace Monero.Shared;

public sealed class MoneroRPCClient
{
    private readonly HttpClient _httpClient;

    public int Port { get; private set; }
    public string Address { get; private set; }

    public MoneroRPCClient(HttpClient httpClient, string address, int port)
    {
        Port = port;
        Address = address;

        _httpClient = httpClient;
    }

    public async Task<BaseResponse<T>?> GetAsync<T>(object request)
    {
        try
        {
            var jsonString = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"http://{Address}:{Port}/json_rpc", jsonString);

            if (!response.IsSuccessStatusCode)
                return default;

            var deserialized = await response.Content.ReadFromJsonAsync<BaseResponse<T>>();

            if (deserialized is null)
                return default;

            if (deserialized.Error is not null)
            {
                Console.WriteLine(deserialized.Error.Message);
            }

            return deserialized;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return default;
        }
    }
}
