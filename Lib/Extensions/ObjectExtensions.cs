using System.Globalization;

namespace Playground.Lib.Extensions
{
    public static class ObjectExtensions
    {
        // value types, returns Nullable<T>
        public static T? NullIfEquals<T>(this T value, T def) where T : struct
        {
            return EqualityComparer<T>.Default.Equals(value, def) ? null : value;
        }

        extension(object? obj)
        {
            public string? ToLowerString()
            {
                return obj?.ToString()?.ToLower();
            }
            public int? ToInt()
            {
                return obj switch
                {
                    int i => i,
                    _ => null
                };
            }

            public string? ToInvariantString()
            {
                return obj switch
                {
                    DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    TimeOnly t => t.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                    DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    _ => Convert.ToString(obj, CultureInfo.InvariantCulture)
                };
            }
        }
    }
}
