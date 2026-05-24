using FluentValidation;

namespace MasarHub.Application.Common.Pagination
{
    public class PaginationValidator<T> : AbstractValidator<T> where T : IPaginatedQuery
    {
        public PaginationValidator()
        {

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithErrorCode("validation.page_number_invalid");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithErrorCode("validation.page_size_invalid")
                .LessThanOrEqualTo(50)
                .WithErrorCode("validation.page_size_too_large");
        }
    }
}
