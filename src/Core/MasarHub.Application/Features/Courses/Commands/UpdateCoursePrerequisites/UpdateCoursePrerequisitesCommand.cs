using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCoursePrerequisites
{
    public sealed record UpdateCoursePrerequisitesCommand(Guid CourseId, List<string> Prerequisites) : IRequest<Result>;
    public sealed record UpdateCoursePrerequisitesRequest(List<string> Prerequisites);
}
