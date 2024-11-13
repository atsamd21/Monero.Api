using System.Text.Json.Serialization;

namespace Monero.Shared;

public class Error
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class BaseResponse<T>
{
    [JsonPropertyName("error")]
    public Error? Error { get; set; }
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("jsonrpc")]
    public string JsonRPC { get; set; } = string.Empty;
    [JsonPropertyName("result")]
    public T? Result { get; set; }
}
