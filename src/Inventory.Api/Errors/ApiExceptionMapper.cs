using Inventory.Application.Authentication;
using Inventory.Application.Categories;
using Inventory.Application.Movements;
using Inventory.Application.Products;
using Inventory.Domain.Categories;
using Inventory.Domain.Movements;
using Inventory.Domain.Products;
namespace Inventory.Api.Errors;
public static class ApiExceptionMapper
{
    public static int GetStatusCode(Exception exception)
    {
        if (exception is CategoryNotFoundException or ProductNotFoundException)
        {

            return StatusCodes.Status404NotFound;
        }
        if (exception is InvalidClientCredentialsException)
        {

            return StatusCodes.Status401Unauthorized;
        }
        if (exception is CategoryHasProductsException or ProductHasMovementsException or DuplicateSkuException or InsufficientStockException)
        {

            return StatusCodes.Status409Conflict;
        }
        if (IsBadRequest(exception))
        {

            return StatusCodes.Status400BadRequest;
        }

        return StatusCodes.Status500InternalServerError;
    }
    public static string GetTitle(Exception exception, int statusCode)
    {
        if (statusCode == StatusCodes.Status500InternalServerError)
        {

            return "An unexpected error occurred.";
        }

        return exception.Message;
    }
    private static bool IsBadRequest(Exception exception)
    {

        return exception is InvalidCategoryNameException or InvalidProductNameException or InvalidSkuException or InvalidPriceException or CategoryReferenceIsRequiredException or QuantityMustBePositiveException or InvalidMovementTypeException or ProductReferenceIsRequiredException or MovementProductMismatchException;
    }
}
