namespace Inventory.Application.Authentication;
public sealed class AccessToken
{
    public AccessToken(string value, DateTime expiresAtUtc)
    {
        Value = value;
        ExpiresAtUtc = expiresAtUtc;
    }
    public string Value { get; }
    public DateTime ExpiresAtUtc { get; }
}
