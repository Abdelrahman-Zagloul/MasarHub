using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.RejectCourse
{
    public sealed record RejectCourseCommand(Guid CourseId, Guid UserId, string Reason) : IRequest<Result>;
    public sealed record RejectCourseRequest(string Reason);

}
