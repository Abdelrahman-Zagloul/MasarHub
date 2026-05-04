using MasarHub.Application.Common.DI;
using System.Data;

namespace MasarHub.Application.Abstractions.Queries
{
    public interface IDbConnectionFactory : IScopedService
    {
        IDbConnection CreateConnection();
    }
}
