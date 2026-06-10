using MasarHub.Application.Common.Models;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.AddLessonAttachment
{
    public sealed record AddLessonAttachmentCommand
    (
        Guid CourseId,
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId,
        FileResource File
    ) : IRequest<Result<AddLessonAttachmentResponse>>;
}
