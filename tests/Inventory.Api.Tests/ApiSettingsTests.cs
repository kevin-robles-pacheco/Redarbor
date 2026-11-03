namespace Inventory.Api.Tests;
public sealed class ApiSettingsTests
{
    [Fact]
    public void FromEnvironment_ReadsConfiguredValues()
    {
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", "local-dev-only-signing-key-32-bytes");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "inventory-api");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "inventory-api");
        Environment.SetEnvironmentVariable("RATE_LIMIT_PERMIT_LIMIT", "25");
        Environment.SetEnvironmentVariable("RATE_LIMIT_WINDOW_SECONDS", "30");
        Environment.SetEnvironmentVariable("OAUTH_RATE_LIMIT_PERMIT_LIMIT", "5");
        Environment.SetEnvironmentVariable("JWT_EXPIRES_IN_SECONDS", "120");
        var settings = ApiSettings.FromEnvironment();
        Assert.Equal(25, settings.PermitLimit);
        Assert.Equal(30, settings.WindowSeconds);
        Assert.Equal(5, settings.OAuthPermitLimit);
        Assert.Equal(120, settings.JwtExpiresInSeconds);
        Assert.Equal("inventory-api", settings.JwtIssuer);
    }
}
