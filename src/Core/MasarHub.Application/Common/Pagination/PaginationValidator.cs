using FluentValidation;

namespace MasarHub.Application.Common.Pagination
{
    public class PaginationValidator<T> : AbstractValidator<T> where T : IPaginatedQuery
    {
        public PaginationValidator()
        {

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(50)
                .WithMessage("Page size must be less than 50");
        }
    }
}
