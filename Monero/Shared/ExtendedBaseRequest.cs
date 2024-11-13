using System.Text.Json.Serialization;

namespace Monero.Shared;

public class ExtendedBaseRequest<TParams> : BaseRequest where TParams : class, new()
{
    [JsonPropertyName("params")]
    public TParams Params { get; set; } = new();

    public ExtendedBaseRequest(string method) : base(method)
    {

    }
}
