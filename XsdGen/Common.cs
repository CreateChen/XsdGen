using System;
using System.Collections.Generic;
using System.Xml;
using XsdAttribute;

namespace XsdGen
{
    public static class Common
    {
        /// <summary>
        /// 参考：https://msdn.microsoft.com/en-us/library/aa719879(v=vs.71).aspx
        /// </summary>
        public static readonly Dictionary<Type, string> TypeMap = new Dictionary<Type, string>
        {
            {typeof (Boolean), "Boolean"},
            {typeof (Byte), "unsignedByte"},
            {typeof (SByte), "Byte"},
            {typeof (DateTime), "dateTime"},
            {typeof (Decimal), "decimal"},
            {typeof (Double), "Double"},
            {typeof (TimeSpan), "duration"},
            {typeof (String[]), "ENTITIES"},
            {typeof (Single), "Float"},
            {typeof (Byte[]), "hexBinary"},
            {typeof (Int16), "short"},
            {typeof (Int32), "int"},
            {typeof (Int64), "long"},
            {typeof (String), "string"},
            {typeof (UInt16), "unsignedShort"},
            {typeof (UInt32), "unsignedInt"},
            {typeof (UInt64), "unsignedLong"},
            {typeof (Uri), "anyURI"},
            {typeof (XmlQualifiedName), "QName"}
        };

        public static string GetXsdTypeName(Type type)
        {
            type = GetPropertyType(type);

            //先从类型字典查找, 没找到, 说明是自定义类型
            if (TypeMap.ContainsKey(type))
                return "xs:" + TypeMap[type];
            return type.Name;
        }

        public static Type GetPropertyType(Type type)
        {
            //数组类型
            if (type.IsArray)
                type = GetArrayType(type);

            //列表类型
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                type = GetGenericType(type);
            }
            return type;
        }

        public static Type GetArrayType(Type type)
        {
            return type.GetElementType();
        }

        public static Type GetGenericType(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        public static string GetFileGroup(Type type)
        {
            type = GetPropertyType(type);

            if (type.IsClass)
                return XsdComplexType.Get(type).FileGroup;
            if (type.IsEnum)
                return XsdSimpleType.Get(type).FileGroup;
            return null;
        }
    }
}