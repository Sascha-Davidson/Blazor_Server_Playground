using Microsoft.AspNetCore.Components;
using System.Web;

namespace Playground.FrontEnd.Base.Functions
{
    public sealed class QueryHelper
    {
        private readonly NavigationManager _navigation;

        public QueryHelper(NavigationManager navigation)
        {
            _navigation = navigation;
        }

        // <summary>
        // Get or set query parameters in one function.
        // - If 'setValues' is provided, updates the URL with those parameters.
        // - Returns the current value of 'key' if specified, or null if key is null.
        // </summary>
        public string QueryParam(string key = null, string defaultValue = null, Dictionary<string, string> setValues = null)
        {
            var uri = _navigation.ToAbsoluteUri(_navigation.Uri);
            var query = HttpUtility.ParseQueryString(uri.Query);

            // Apply new parameters if provided
            if (setValues != null)
            {
                foreach (var kvp in setValues)
                    query[kvp.Key] = kvp.Value;

                var baseUri = uri.GetLeftPart(UriPartial.Path);
                var newUri = $"{baseUri}?{query}";
                _navigation.NavigateTo(newUri, forceLoad: false);
            }

            // Return requested key or null
            if (key != null)
                return query[key] ?? defaultValue;

            return null;
        }
    }
}
