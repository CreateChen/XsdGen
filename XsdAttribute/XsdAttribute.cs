using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace XsdAttribute
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class XsdSchema : Attribute
    {
        public string TargetNamespace { get; set; }
        public string XmlNamespace { get; set; }
        public string Namespace { get; set; }
        public string Common { get; set; }

        //汇总dll中的类型，生成总的xsd
        private string _packageId;
        /// <summary>
        /// 生成XSD的文件名称
        /// </summary>
        public string PackageId
        {
            get { return _packageId; }
            set
            {
                //去除文件名中非法字符
                var regex = new Regex(@"\:|\;|\/|\\|\||\,|\*|\?|\""|\<|\>");
                _packageId = regex.Replace(value, "");
            }
        }

        public static XsdSchema Get(Assembly assembly)
        {
            return (XsdSchema) GetCustomAttribute(assembly, typeof (XsdSchema));
        }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class XsdImport : Attribute
    {
        public string Id { get; set; }
        public string SchemaLocation { get; set; }
        public string Namespace { get; set; }

        public static IEnumerable<XsdImport> Get(Assembly assembly)
        {
            var attributes = GetCustomAttributes(assembly, typeof (XsdImport));
            return attributes.Cast<XsdImport>();
        }
    }

    [AttributeUsage((AttributeTargets.Class))]
    public class XsdComplexType : Attribute
    {
        private string _fileGroup;

        /// <summary>
        /// 生成XSD的文件名称
        /// </summary>
        public string FileGroup
        {
            get { return _fileGroup; }
            set
            {
                //去除文件名中非法字符
                var regex = new Regex(@"\:|\;|\/|\\|\||\,|\*|\?|\""|\<|\>");
                _fileGroup = regex.Replace(value, "");
            }
        }

        public string Annotation { get; set; }

        public static XsdComplexType Get(Type type)
        {
            return (XsdComplexType) GetCustomAttribute(type, typeof (XsdComplexType));
        }
    }

    [AttributeUsage((AttributeTargets.Property))]
    public class XsdElement : Attribute
    {
        public string MinOccurs { get; set; }
        public string MaxOccurs { get; set; }
        public string Annotation { get; set; }

        public static XsdElement Get(PropertyInfo propertyInfo)
        {
            return (XsdElement) GetCustomAttribute(propertyInfo, typeof (XsdElement));
        }
    }

    [AttributeUsage((AttributeTargets.Enum))]
    public class XsdSimpleType : Attribute
    {
        private string _fileGroup;

        /// <summary>
        /// 生成XSD的文件名称
        /// </summary>
        public string FileGroup
        {
            get { return _fileGroup; }
            set
            {
                //去除文件名中非法字符
                var regex = new Regex(@"\:|\;|\/|\\|\||\,|\*|\?|\""|\<|\>");
                _fileGroup = regex.Replace(value, "");
            }
        }

        public string Annotation { get; set; }

        public static XsdSimpleType Get(Type type)
        {
            return (XsdSimpleType) GetCustomAttribute(type, typeof (XsdSimpleType));
        }
    }
}