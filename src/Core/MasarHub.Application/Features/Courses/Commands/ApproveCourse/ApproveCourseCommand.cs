using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.ApproveCourse
{
    public sealed record ApproveCourseCommand(Guid CourseId, Guid UserId) : IRequest<Result>;
}
