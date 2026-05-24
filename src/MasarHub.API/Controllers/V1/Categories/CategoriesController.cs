using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Common.Constants;
using MasarHub.Application.Features.Categories.Commands.CreateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1.Categories
{
    [ApiVersion(1.0)]
    [Tags("Categories")]
    [Route("api/categories")]
    public sealed class CategoriesController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public CategoriesController(ILocalizationService localizationService, IMediator mediator)
            : base(localizationService)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return Created($"{GetBaseUrl()}/{result.Value.Id}", new
            {
                Message = await _localizationService.GetAsync("category.created"),
                Category = result.Value
            });
        }
    }
}
