namespace MasarHub.Application.Features.Modules.Queries.GetModuleById
{
    public sealed record ModuleDetailsResponse
    (
        Guid ModuleId,
        string Title,
        string? Description,
        int DisplayOrder
    )
    {
        public List<LessonResponse> Lessons { get; set; } = [];
    }

    public sealed record LessonResponse
    (
        Guid LessonId,
        string Title,
        string? Description,
        int DisplayOrder,
        bool IsPreviewable,
        string LessonType,
        string? VideoPublicId
    )
    {
        public string? VideoUrl { get; set; }
    }
}
