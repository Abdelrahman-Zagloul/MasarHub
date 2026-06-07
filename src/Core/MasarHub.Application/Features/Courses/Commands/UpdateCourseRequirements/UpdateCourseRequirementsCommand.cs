using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseRequirements
{
    public sealed record UpdateCourseRequirementsCommand(Guid CourseId, List<string> Requirements) : IRequest<Result>;
    public sealed record UpdateCourseRequirementsRequest(List<string> Requirements);
}
