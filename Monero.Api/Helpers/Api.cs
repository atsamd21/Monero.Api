using System.Text.Json;
using System.Text;
using Monero.Api.Models;

namespace Monero.Api.Helpers;

public static class Api
{
    public static async Task<Response<T>> PutToApiAsync<T>(this HttpClient httpClient, object request, string endpoint, string token)
    {
        Response<T> response = new();

        var httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        for (int i = 0; i < 3; i++)
        {
            try
            {
                // 
                httpClient.DefaultRequestHeaders.Clear();
                //
                httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var apiResponse = await httpClient.PutAsync(endpoint, httpContent);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    await Task.Delay(500);
                    continue;
                }

                var deserialized = await apiResponse.Content.ReadFromJsonAsync<T>();

                if (deserialized is null)
                {
                    response.Message = apiResponse.ReasonPhrase ?? "Error";
                    return response;
                }

                response.Value = deserialized;
                return response;
            }
            catch
            {
                await Task.Delay(500);
            }
        }

        response.Message = "Error.";
        return response;
    }

    public static async Task<Response<T>> PostToApiAsync<T>(this HttpClient httpClient, object request, string endpoint, string token)
    {
        Response<T> response = new();

        var httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        for (int i = 0; i < 3; i++)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                var apiResponse = await httpClient.PostAsync(endpoint, httpContent);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    await Task.Delay(500);
                    continue;
                }

                var deserialized = await apiResponse.Content.ReadFromJsonAsync<T>();

                if (deserialized is null)
                {
                    response.Message = apiResponse.ReasonPhrase ?? "Error";
                    return response;
                }

                response.Value = deserialized;
                return response;
            }
            catch
            {
                await Task.Delay(500);
            }
        }

        response.Message = "Error.";
        return response;
    }

    public static async Task<Response<T>> GetFromApiAsync<T>(this HttpClient httpClient, string endpoint, string token)
    {
        Response<T> response = new();

        for (int i = 0; i < 3; i++)
        {
            try
            {
                // Move to program.cs
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var apiResponse = await httpClient.GetAsync(endpoint);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    await Task.Delay(500);
                    continue;
                }

                var deserialized = await apiResponse.Content.ReadFromJsonAsync<T>();

                if (deserialized is null)
                {
                    response.Message = apiResponse.ReasonPhrase ?? "Error";
                    return response;
                }

                response.Value = deserialized;
                return response;
            }
            catch
            {
                await Task.Delay(500);
            }
        }

        response.Message = "Error.";
        return response;
    }
}
