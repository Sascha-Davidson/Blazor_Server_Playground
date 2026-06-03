using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Playground.Services
{
    public class UserAccountCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var user = httpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
                return Task.FromResult<ProviderCultureResult?>(null);

            var cultureClaim = user.FindFirst("culture")?.Value;
            if (string.IsNullOrEmpty(cultureClaim))
                return Task.FromResult<ProviderCultureResult?>(null);

            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(cultureClaim));
        }
    }
}