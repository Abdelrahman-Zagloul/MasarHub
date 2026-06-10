using MasarHub.API.Extensions;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace MasarHub.API.Controllers.Shared
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting(RateLimitingPolicies.Global)]
    public abstract class ApiBaseController : ControllerBase
    {
        protected readonly ILocalizationService _localizationService;

        public ApiBaseController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        protected Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        protected string GetBaseUrl() => $"{Request.Scheme}://{Request.Host}{Request.Path}";
        protected Dictionary<string, string> GetQueryParams() => Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

        protected async Task<IActionResult> HandleError(Result result)
            => await Errors.ProblemDetailsFactory.CreateAsync(result, this, _localizationService);
        protected async Task<OkObjectResult> SuccessMessage(string code)
            => Ok(new { Message = await _localizationService.GetAsync(code) });


        protected async Task<IActionResult> ToOkResultAsync<T>(Result<T> result)
        {
            return result.IsFailure
                ? await HandleError(result)
                : Ok(result.Value);
        }
        protected async Task<IActionResult> ToOkResultAsync<T>(Result<T> result, object customData)
        {
            return result.IsFailure
                ? await HandleError(result)
                : Ok(customData);
        }
        protected async Task<IActionResult> ToNoContentResultAsync(Result result)
        {
            return result.IsFailure
                ? await HandleError(result)
                : NoContent();
        }
    }
}
