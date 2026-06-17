using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.ToggleExamPublished
{
    public sealed record ToggleExamPublishedCommand(Guid ExamId, Guid InstructorId, bool IsPublished) : IRequest<Result>;
}