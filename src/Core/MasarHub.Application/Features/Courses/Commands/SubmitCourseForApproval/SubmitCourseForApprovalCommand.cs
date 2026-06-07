using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.SubmitCourseForApproval
{
    public sealed record SubmitCourseForApprovalCommand(Guid CourseId, Guid InstructorId) : IRequest<Result>;
}
