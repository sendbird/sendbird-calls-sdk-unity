// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    internal static class NewtonsoftJsonExtension
    {
        internal static string EnumToJsonPropertyName<TEnumType>(TEnumType enumValue) where TEnumType : Enum
        {
            Type enumType = typeof(TEnumType);
            string enumName = Enum.GetName(enumType, enumValue);
            FieldInfo fieldInfo = enumType.GetField(enumName);
            if (fieldInfo != null)
            {
                JsonPropertyAttribute attribute = fieldInfo.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute != null && attribute.PropertyName != null) return attribute.PropertyName;
            }
    
            return enumName;
        }
    }
}