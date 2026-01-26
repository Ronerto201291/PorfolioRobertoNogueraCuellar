using System;

namespace PruebaAngular.Application.Extensions
{
    public static class TypeExtensions
    {
        public static string GetGenericTypeName(this object @object)
        {
            return @object?.GetType().GetGenericTypeName() ?? string.Empty;
        }

        public static string GetGenericTypeName(this Type type)
        {
            if (type == null) return string.Empty;

            var typeName = type.Name;

            if (type.IsGenericType)
            {
                var genericTypeNames = string.Join(",", Array.ConvertAll(type.GetGenericArguments(), t => t.Name));
                typeName = $"{typeName.Remove(typeName.IndexOf('`'))}<{genericTypeNames}>";
            }

            return typeName;
        }
    }
}
