using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IExamQuery : IScopedService
    {
        Task<ExamCreationData> GetCreationDataAsync(Guid courseId, Guid? moduleId, Guid instructorId, CancellationToken ct = default);
        Task<ExamUpdateData> GetUpdateDataAsync(Guid examId, Guid instructorId, CancellationToken ct = default);
        Task<ExamState> GetExamStateAsync(Guid examId, Guid instructorId, CancellationToken ct = default);
        Task<Exam?> GetExamDetailsAsync(Guid examId, CancellationToken ct = default);
        Task<ExamAttemptStartData?> GetAttemptStartDataAsync(Guid examId, Guid courseId, Guid userId, CancellationToken ct = default);
    }
}

public sealed record ExamCreationData(bool CourseExists, bool IsOwner, bool ModuleExists);
public sealed record ExamUpdateData(bool ExamExists, bool IsOwner, bool IsPublished);
public sealed record ExamState(bool ExamExists, bool IsOwner, bool HasAttempts);
public sealed record ExamAttemptStartData(bool IsEnrolled, int CompletedAttempts, Guid? InProgressAttemptId);
