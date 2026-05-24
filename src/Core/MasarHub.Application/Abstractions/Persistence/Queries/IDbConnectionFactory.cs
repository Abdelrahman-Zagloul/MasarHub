using MasarHub.Application.Common.DependencyInjection;
using System.Data;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IDbConnectionFactory : IScopedService
    {
        IDbConnection CreateConnection();
    }
}
