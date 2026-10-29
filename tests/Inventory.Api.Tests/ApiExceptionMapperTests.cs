using Inventory.Api.Contracts;
using Inventory.Api.Errors;
using Inventory.Application.Authentication;
using Inventory.Application.Categories;
using Inventory.Domain.Categories;
using Inventory.Domain.Movements;
namespace Inventory.Api.Tests;
public sealed class ApiExceptionMapperTests
{
    [Fact]
    public void GetStatusCode_WhenCategoryIsMissing_ReturnsNotFound()
    {
        var statusCode = ApiExceptionMapper.GetStatusCode(new CategoryNotFoundException());
        Assert.Equal(404, statusCode);
    }
    [Fact]
    public void GetStatusCode_WhenCredentialsAreInvalid_ReturnsUnauthorized()
    {
        var statusCode = ApiExceptionMapper.GetStatusCode(new InvalidClientCredentialsException());
        Assert.Equal(401, statusCode);
    }
    [Fact]
    public void GetStatusCode_WhenStockIsInsufficient_ReturnsConflict()
    {
        var statusCode = ApiExceptionMapper.GetStatusCode(new InsufficientStockException(1, 2));
        Assert.Equal(409, statusCode);
    }
    [Fact]
    public void GetStatusCode_WhenCategoryNameIsInvalid_ReturnsBadRequest()
    {
        var statusCode = ApiExceptionMapper.GetStatusCode(new InvalidCategoryNameException());
        Assert.Equal(400, statusCode);
    }
    [Fact]
    public void GetTitle_WhenErrorIsUnexpected_HidesExceptionMessage()
    {
        var exception = new InvalidOperationException("POSTGRES_CONNECTION_STRING");
        var statusCode = ApiExceptionMapper.GetStatusCode(exception);
        var title = ApiExceptionMapper.GetTitle(exception, statusCode);
        Assert.Equal(500, statusCode);
        Assert.Equal("An unexpected error occurred.", title);
    }
    [Fact]
    public void Parse_WhenTypeIsIn_ReturnsIn()
    {
        var movementType = MovementTypeParser.Parse("in");
        Assert.Equal(MovementType.In, movementType);
    }
    [Fact]
    public void Parse_WhenTypeIsUnknown_ThrowsInvalidMovementTypeException()
    {
        Assert.Throws<InvalidMovementTypeException>(() => MovementTypeParser.Parse("transfer"));
    }
}
