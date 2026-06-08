namespace MasarHub.Application.Features.Modules.Commands.CreateModule
{
    public record CreateModuleResponse
    (
        Guid Id,
        Guid CourseId,
        string Title,
        int DisplayOrder,
        string? Description
    );
}
