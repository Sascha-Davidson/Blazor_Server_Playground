using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;

namespace Playground.Lib.Extensions
{
    public static class TypeExtensions
    {
        extension(MemberInfo memberInfo)
        {
            public string? GetDisplayValue()
            {
                var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute == null)
                {
                    return memberInfo switch
                    {
                        PropertyInfo p => p.Name,
                        FieldInfo f => f.GetValue(null)?.ToString(), // For enums
                        _ => memberInfo.Name
                    };
                }

                var resourceManager = (ResourceManager)displayAttribute.ResourceType?.GetProperty("ResourceManager")?.GetValue(null)!;
                var translatedString = resourceManager.GetString(displayAttribute.Name ?? "");

                return translatedString != null && translatedString.IsNullOrEmpty() ? displayAttribute.Name : translatedString;
            }
        }
    }
}
