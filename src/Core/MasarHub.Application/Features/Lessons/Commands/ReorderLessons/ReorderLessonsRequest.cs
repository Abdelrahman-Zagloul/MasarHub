namespace MasarHub.Application.Features.Lessons.Commands.ReorderLessons
{
    public sealed record ReorderLessonsRequest(IReadOnlyList<Guid> OrderedLessonIds);

}
