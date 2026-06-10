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
        private readonly ISender _sender;

        public CategoriesController(ILocalizationService localizationService, ISender sender)
            : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetCategoryById), new { id = result.Value.Id }, result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategorizes([FromQuery] GetCategoriesQuery query)
        {
            var result = await _sender.Send(query);
            return await ToOkResultAsync(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _sender.Send(new GetCategoryByIdQuery(id));
            return await ToOkResultAsync(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryRequest request)
        {
            var result = await _sender.Send(new UpdateCategoryCommand(id, request.Name, request.Description, request.ParentCategoryId, request.MoveToRoot));
            return await ToNoContentResultAsync(result);
        }

        [HttpPatch("reorder")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ReorderCategories(ReorderCategoriesCommand command)
        {
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _sender.Send(new DeleteCategoryCommand(id));
            return await ToNoContentResultAsync(result);
        }
    }
}
