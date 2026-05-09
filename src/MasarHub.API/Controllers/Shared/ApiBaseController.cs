using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MasarHub.API.Controllers.Shared
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        private readonly ILocalizationService _localizationService;

        public ApiBaseController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        protected Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        protected string GetBaseUrl() => $"{Request.Scheme}://{Request.Host}{Request.Path}";
        protected Dictionary<string, string> GetQueryParams() => Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

        protected async Task<IActionResult> HandleError(Result result)
            => await Common.ProblemDetailsFactory.CreateAsync(result, this, _localizationService);
        protected async Task<OkObjectResult> SuccessMessage(string code)
            => Ok(new { Message = await _localizationService.GetAsync(code) });
    }
}
