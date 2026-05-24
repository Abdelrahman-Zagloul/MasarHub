using System.Text.RegularExpressions;

namespace MasarHub.Application.Common.Helpers
{
    public static class SlugGenerator
    {
        public static string GenerateSlug(string value)
        {
            var slug = value.Trim().ToLowerInvariant();

            slug = Regex.Replace(slug, @"[^a-z0-9\u0600-\u06FF]+", "-");
            slug = Regex.Replace(slug, "-{2,}", "-").Trim('-');

            return string.IsNullOrWhiteSpace(slug)
                ? Guid.CreateVersion7().ToString("N")
                : slug;
        }
    }
}