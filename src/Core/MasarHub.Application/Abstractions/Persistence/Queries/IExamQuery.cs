using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IExamQuery : IScopedService
    {
        Task<ExamCreationData> GetCreationDataAsync(Guid courseId, Guid? moduleId, Guid instructorId, CancellationToken ct = default);
    }
}

public sealed record ExamCreationData(bool CourseExists, bool IsOwner, bool ModuleExists);
