namespace MasarHub.Application.Common.Pagination
{
    public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
}
