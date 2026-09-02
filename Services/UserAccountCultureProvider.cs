using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Playground.Services
{
    public class UserAccountCultureProvider(
        IOptions<RequestLocalizationOptions> options) : IRequestCultureProvider
    {
        private readonly RequestLocalizationOptions _options = options.Value;

        public Task<ProviderCultureResult?> DetermineProviderCultureResult(
            HttpContext httpContext)
        {
            ClaimsPrincipal? user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Task.FromResult<ProviderCultureResult?>(null);

            string? culture = user.FindFirst("culture")?.Value;

            if (string.IsNullOrWhiteSpace(culture))
                return Task.FromResult<ProviderCultureResult?>(null);

            bool? isSupported = _options.SupportedCultures?
                .Any(x => x.Name.Equals(culture, StringComparison.OrdinalIgnoreCase));

            if (isSupported != true)
                return Task.FromResult<ProviderCultureResult?>(null);

            return Task.FromResult<ProviderCultureResult?>(
                new ProviderCultureResult(culture));
        }
    }
}