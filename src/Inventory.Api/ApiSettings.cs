namespace Inventory.Api;
public sealed class ApiSettings
{
    private ApiSettings(string jwtSigningKey, string jwtIssuer, string jwtAudience, int permitLimit, int windowSeconds, int oauthPermitLimit, int jwtExpiresInSeconds)
    {
        JwtSigningKey = jwtSigningKey;
        JwtIssuer = jwtIssuer;
        JwtAudience = jwtAudience;
        PermitLimit = permitLimit;
        WindowSeconds = windowSeconds;
        OAuthPermitLimit = oauthPermitLimit;
        JwtExpiresInSeconds = jwtExpiresInSeconds;
    }
    public string JwtSigningKey { get; }
    public string JwtIssuer { get; }
    public string JwtAudience { get; }
    public int PermitLimit { get; }
    public int WindowSeconds { get; }
    public int OAuthPermitLimit { get; }
    public int JwtExpiresInSeconds { get; }
    public static ApiSettings FromEnvironment()
    {
        var settings = new ApiSettings(ReadRequired("JWT_SIGNING_KEY"), ReadRequired("JWT_ISSUER"), ReadRequired("JWT_AUDIENCE"), ReadInt("RATE_LIMIT_PERMIT_LIMIT", 100), ReadInt("RATE_LIMIT_WINDOW_SECONDS", 60), ReadInt("OAUTH_RATE_LIMIT_PERMIT_LIMIT", 10), ReadInt("JWT_EXPIRES_IN_SECONDS", 3600));

        return settings;
    }
    private static string ReadRequired(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{name} is required.");
        }

        return value;
    }
    private static int ReadInt(string name, int fallback)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (int.TryParse(value, out var parsed))
        {

            return parsed;
        }

        return fallback;
    }
}
