using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Inventory.Api.Tests.Support;
namespace Inventory.Api.Tests;
public sealed class InventoryApiIntegrationTests : IClassFixture<InventoryApiFactory>
{
    private readonly HttpClient _client;
    public InventoryApiIntegrationTests(InventoryApiFactory factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task Token_WithValidCredentials_ReturnsBearerToken()
    {
        var token = await RequestTokenAsync();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }
    [Fact]
    public async Task Token_WithInvalidSecret_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync("/oauth/token", CreateTokenContent("inventory-api", "wrong"));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task Categories_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task InventoryFlow_CreateCategoryProductAndMovement_Succeeds()
    {
        var token = await RequestTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", new { name = "Beverages", description = "Drinks" });
        Assert.Equal(HttpStatusCode.Created, categoryResponse.StatusCode);
        var category = await categoryResponse.Content.ReadFromJsonAsync<JsonElement>();
        var categoryId = category.GetProperty("id").GetGuid();
        var productResponse = await _client.PostAsJsonAsync("/api/products", new { name = "Water", sku = "WATER-1", price = 1.5m, categoryId });
        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);
        var product = await productResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0, product.GetProperty("quantityOnHand").GetInt32());
        var productId = product.GetProperty("id").GetGuid();
        var movementResponse = await _client.PostAsJsonAsync("/api/inventory-movements", new { productId, type = "In", quantity = 5 });
        Assert.Equal(HttpStatusCode.OK, movementResponse.StatusCode);
        var updated = await movementResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(5, updated.GetProperty("quantityOnHand").GetInt32());
    }
    [Fact]
    public async Task InventoryFlow_WhenOutboundExceedsStock_ReturnsConflict()
    {
        var token = await RequestTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", new { name = "Tools", description = (string?)null });
        var category = await categoryResponse.Content.ReadFromJsonAsync<JsonElement>();
        var categoryId = category.GetProperty("id").GetGuid();
        var productResponse = await _client.PostAsJsonAsync("/api/products", new { name = "Hammer", sku = "HAMMER-1", price = 9.9m, categoryId });
        var product = await productResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productId = product.GetProperty("id").GetGuid();
        await _client.PostAsJsonAsync("/api/inventory-movements", new { productId, type = "In", quantity = 1 });
        var outbound = await _client.PostAsJsonAsync("/api/inventory-movements", new { productId, type = "Out", quantity = 5 });
        Assert.Equal(HttpStatusCode.Conflict, outbound.StatusCode);
    }
    private async Task<string> RequestTokenAsync()
    {
        var response = await _client.PostAsync("/oauth/token", CreateTokenContent("inventory-api", "local-dev-only"));
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();

        return payload.GetProperty("access_token").GetString() ?? string.Empty;
    }
    private static FormUrlEncodedContent CreateTokenContent(string clientId, string clientSecret)
    {
        var values = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        };

        return new FormUrlEncodedContent(values);
    }
}
