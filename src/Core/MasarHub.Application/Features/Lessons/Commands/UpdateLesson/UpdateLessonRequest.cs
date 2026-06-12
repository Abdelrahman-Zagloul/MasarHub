namespace MasarHub.Application.Features.Lessons.Commands.UpdateLesson
{
    public sealed record UpdateLessonRequest
    (
        string? Title,
        string? Description
    );
}
