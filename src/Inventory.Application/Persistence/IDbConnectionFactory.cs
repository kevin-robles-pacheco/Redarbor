using System.Data;
namespace Inventory.Application.Persistence;
public interface IDbConnectionFactory
{
    IDbConnection OpenConnection();
}
