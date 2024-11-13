namespace Monero.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthorizeAttribute : Attribute
{
    public ApiKeyAuthorizeAttribute()
    {

    }
}
