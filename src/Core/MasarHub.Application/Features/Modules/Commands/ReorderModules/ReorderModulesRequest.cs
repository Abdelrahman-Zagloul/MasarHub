namespace MasarHub.Application.Features.Modules.Commands.ReorderModules
{
    public sealed record ReorderModulesRequest(IReadOnlyList<Guid> OrderedModuleIds);
}
