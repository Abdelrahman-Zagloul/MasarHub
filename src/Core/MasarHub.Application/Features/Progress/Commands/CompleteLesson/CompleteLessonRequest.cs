namespace MasarHub.Application.Features.Progress.Commands.CompleteLesson
{
    public sealed record CompleteLessonRequest(Guid CourseId, Guid LessonId);
}
