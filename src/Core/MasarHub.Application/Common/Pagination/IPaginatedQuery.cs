namespace MasarHub.Application.Common.Pagination
{
    public interface IPaginatedQuery
    {
        int PageNumber { get; }
        int PageSize { get; }
    }
}
