using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseLearningObjective
{
    public sealed record UpdateCourseLearningObjectiveCommand(Guid CourseId, List<string> LearningObjectives) : IRequest<Result>;
    public sealed record UpdateCourseLearningObjectiveRequest(List<string> LearningObjectives);
}
