using MasarHub.Application.Settings;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace MasarHub.API.Middlewares;

public class CultureMiddleware : IMiddleware
{
    private readonly LocalizationSettings _settings;

    public CultureMiddleware(IOptions<LocalizationSettings> options)
    {
        _settings = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var cultureName = GetCultureName(context);

        var culture = SafeGetCulture(cultureName);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        await next(context);
    }

    private string GetCultureName(HttpContext context)
    {
        var header = context.Request.Headers["Accept-Language"].ToString();
        if (string.IsNullOrWhiteSpace(header))
            return _settings.DefaultCulture;

        var culture = header.Split(',').FirstOrDefault();
        return string.IsNullOrWhiteSpace(culture) ? _settings.DefaultCulture : culture;
    }
    private CultureInfo SafeGetCulture(string culture)
    {
        try
        {
            if (_settings.SupportedCultures is not null && !_settings.SupportedCultures.Contains(culture))
            {
                return CultureInfo.GetCultureInfo(_settings.DefaultCulture);
            }

            return CultureInfo.GetCultureInfo(culture);
        }
        catch
        {
            return CultureInfo.GetCultureInfo(_settings.DefaultCulture);
        }
    }
}