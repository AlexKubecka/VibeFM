using System;
using System.ComponentModel;
using System.Reflection;

namespace FootballManager.Utilities
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(T enumValue) where T : Enum
        {
            FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null) return enumValue.ToString(); // Handle null FieldInfo

            DescriptionAttribute? attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? enumValue.ToString(); // Handle null DescriptionAttribute
        }
    }
}