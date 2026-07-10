using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Modules.Queries.GetModuleById
{
    public sealed class GetModuleByIdQueryValidator : AbstractValidator<GetModuleByIdQuery>
    {
        public GetModuleByIdQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");
        }
    }
}
