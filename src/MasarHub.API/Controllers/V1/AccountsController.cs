using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Accounts.Commands.ApproveInstructor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Accounts")]
    [Route("api/v{version:apiVersion}/accounts")]
    public sealed class AccountsController : ApiControllerBase
    {
        private readonly ISender _sender;
        public AccountsController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPut("{instructorUserId:guid}/approve")]
        [Authorize(Roles = Roles.Admin)]
        [EndpointSummary("Approve an instructor account")]
        [EndpointDescription("Approves a pending instructor account by user ID. Admin only.")]
        public async Task<IActionResult> ApproveInstructor(Guid instructorUserId)
        {
            var result = await _sender.Send(new ApproveInstructorCommand(instructorUserId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }


    }
}
