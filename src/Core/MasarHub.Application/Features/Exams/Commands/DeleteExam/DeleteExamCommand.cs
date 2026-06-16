using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.DeleteExam
{
    public sealed record DeleteExamCommand(Guid ExamId, Guid InstructorId) : IRequest<Result>;
}
