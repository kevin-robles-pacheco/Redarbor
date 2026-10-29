namespace Inventory.Application.Authentication;
public sealed class InvalidClientCredentialsException : Exception
{
    public InvalidClientCredentialsException()
        : base("Client credentials are invalid.")
    {
    }
}
