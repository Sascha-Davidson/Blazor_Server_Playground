using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Playground.Lib.Extensions
{
    public static class EnumExtensions
    {
        extension(Enum value)
        {
            public string ToCssClass()
            {
                var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();

                if (member == null)
                    return string.Empty;

                var attr = member.GetCustomAttribute<CssClassAttribute>();

                return string.IsNullOrWhiteSpace(attr?.ClassName) ? string.Empty : attr.ClassName;
            }

            public string GetDisplayName()
            {
                var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();

                if (member == null)
                    return $"[Undefined: {value}]";

                var attr = member.GetCustomAttribute<DisplayAttribute>();

                if (attr == null)
                    return value.ToString();

                if (attr.ResourceType == null)
                    return attr.Name ?? value.ToString();

                var prop = attr.ResourceType.GetProperty(attr.Name);
                if (prop != null)
                    return prop.GetValue(null)?.ToString() ?? value.ToString();

                return attr.Name ?? value.ToString();
            }
        }

        extension<T>(IEnumerable<T> items)
        {
            public string JoinAsString(string separator = " ")
            {
#pragma warning disable DebitanW002
                return string.Join(separator, items);
#pragma warning restore DebitanW002
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CssClassAttribute(string className) : Attribute
    {
        public string ClassName { get; } = className;
    }
}