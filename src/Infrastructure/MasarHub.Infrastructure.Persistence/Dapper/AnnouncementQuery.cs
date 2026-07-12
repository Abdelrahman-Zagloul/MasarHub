using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class AnnouncementQuery : IAnnouncementQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AnnouncementQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Guid>> GetEnrolledUserIdsAsync(Guid courseId, CancellationToken ct)
        {
            const string sql = @"
                SELECT UserId
                FROM courses.CourseEnrollments
                WHERE CourseId = @CourseId AND IsDeleted = 0 AND Status = 'Active'";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: ct);
            return (await connection.QueryAsync<Guid>(command)).AsList();
        }
    }
}
