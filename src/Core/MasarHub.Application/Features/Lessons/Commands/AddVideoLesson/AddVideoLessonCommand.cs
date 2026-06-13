using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.AddVideoLesson
{
    public sealed record AddVideoLessonCommand
    (
        Guid ModuleId,
        Guid InstructorId,
        bool IsPreviewable,
        string Title,
        string? Description,
        string FileKey
    ) : IRequest<Result<AddVideoLessonResponse>>;
}
