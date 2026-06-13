using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.CompleteVideoLesson
{
    public sealed record CompleteVideoLessonCommand
    (
        Guid ModuleId,
        Guid InstructorId,
        bool IsPreviewable,
        string Title,
        string? Description,
        string FileKey
    ) : IRequest<Result<CompleteVideoLessonResponse>>;
}
