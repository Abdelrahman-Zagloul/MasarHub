using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Categories.Queries.GetCategoryById
{
    public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryWithChildrenResponse>>;
}
