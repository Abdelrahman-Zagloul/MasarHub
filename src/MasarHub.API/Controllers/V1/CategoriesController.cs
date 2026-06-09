using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Categories.Commands.CreateCategory;
using MasarHub.Application.Features.Categories.Commands.DeleteCategory;
using MasarHub.Application.Features.Categories.Commands.ReorderCategories;
using MasarHub.Application.Features.Categories.Commands.UpdateCategory;
using MasarHub.Application.Features.Categories.Queries.GetCategories;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
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

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetCategoriesQuery query)
        {
            var result = await _mediator.Send(query);
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id));
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryRequest request)
        {
            var result = await _mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Description, request.ParentCategoryId, request.MoveToRoot));
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }

        [HttpPatch("reorder")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ReorderCategories(ReorderCategoriesCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("category.deleted");
        }
    }

}
