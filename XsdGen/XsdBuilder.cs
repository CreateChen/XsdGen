using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using XsdAttribute;

namespace XsdGen
{
    public class XsdBuilder
    {
        private readonly XNamespace _xs = "http://www.w3.org/2001/XMLSchema";
        private Assembly _assembly;
        private Dictionary<string, XsdFile> _xsdFiles;
        private List<XElement> _elements;

        public void Build(Assembly assembly)
        {
            _assembly = assembly;
            _xsdFiles = new Dictionary<string, XsdFile>();
            _elements = new List<XElement>();

            XElement[] defaultImports = GetDefaultImports(_assembly).ToArray();
            XsdSchema defaultSchema = XsdSchema.Get(_assembly);

            string directoryName = defaultSchema.PackageId ?? _assembly.FullName;
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            BuildTypes(_assembly);
            foreach (var item in _xsdFiles)
            {
                item.Value.Schema = GetSchema(defaultSchema, item.Key);
                foreach (var import in defaultImports)
                {
                    item.Value.Imports.Add(import);
                }
                //生成XSD文件
                string fileName = string.Format("{0}/{1}.xsd", directoryName, item.Key);
                item.Value.ToXML().Save(fileName);
                Console.WriteLine("Generate {0}", fileName);
            }
            //生成汇总XSD文件
            var summaryXsdFile = BuildSummary(defaultSchema, defaultImports);
            string summaryXsdFileName = string.Format("{0}/{1}.xsd", directoryName, directoryName);
            summaryXsdFile.ToXML().Save(summaryXsdFileName);
            Console.WriteLine(string.Format("{0}Generate Summary{0}\n{1}", new String('=', 10), summaryXsdFileName));
        }

        private XElement GetSchema(XsdSchema xsdSchema, string id)
        {
            var schema = new XElement(
                _xs + "schema",
                new XAttribute("id", id),
                new XAttribute("targetNamespace", xsdSchema.TargetNamespace),
                new XAttribute("elementFormDefault", "qualified"),
                new XAttribute("attributeFormDefault", "unqualified"),
                new XAttribute(XNamespace.Xmlns + "xs", _xs.ToString())
                );
            if (!string.IsNullOrEmpty(xsdSchema.XmlNamespace))
                schema.SetAttributeValue("xmlns", xsdSchema.XmlNamespace);
            if (!string.IsNullOrEmpty(xsdSchema.Namespace))
                schema.SetAttributeValue(XNamespace.Xmlns + "ns", xsdSchema.Namespace);
            if (!string.IsNullOrEmpty(xsdSchema.Common))
                schema.SetAttributeValue(XNamespace.Xmlns + "common", xsdSchema.Common);
            return schema;
        }

        private IEnumerable<XElement> GetDefaultImports(Assembly assembly)
        {
            var xsdImports = XsdImport.Get(assembly);

            return xsdImports.Select(xsdImport => new XElement(
                _xs + "import",
                new XAttribute("id", xsdImport.Id),
                new XAttribute("schemaLocation", xsdImport.SchemaLocation),
                new XAttribute("namespace", xsdImport.Namespace)
                ));
        }

        private void BuildTypes(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                string fileGroup;
                if (type.IsClass)
                {
                    var element = BuildElement(type);
                    _elements.Add(element);
                    //_xsdFiles[fileGroup].Elements.Add(element);

                    var complexTypeElement = BuildComplexType(type, out fileGroup);
                    _xsdFiles[fileGroup].Elements.Add(complexTypeElement);
                }
                else if (type.IsEnum)
                {
                    var simpleTypeElement = BuildSimpleType(type, out fileGroup);
                    _xsdFiles[fileGroup].Elements.Add(simpleTypeElement);
                }
            }
        }

        public XElement BuildElement(Type type)
        {
            //只有Request或者Response对象类型，末尾自动添加Type
            string name = (type.Name.EndsWith("Request") || type.Name.EndsWith("Response"))
                ? type.Name + "Type"
                : type.Name;

            return new XElement(
                _xs + "element",
                new XAttribute("name", type.Name),
                    new XAttribute("nillable", true),
                new XAttribute("type", "ns:" + name)
                );
        }

        private XElement BuildComplexType(Type type, out string fileGroup)
        {
            var xsdComplexType = XsdComplexType.Get(type);
            //添加XSD文件
            fileGroup = xsdComplexType.FileGroup;
            SetDefaultFile(fileGroup);

            //只有Request或者Response对象类型，末尾自动添加Type
            string name = (type.Name.EndsWith("Request") || type.Name.EndsWith("Response"))
                ? type.Name + "Type"
                : type.Name;

            var complexTypeElement = new XElement(
                _xs + "complexType",
                new XAttribute("name", name)
                );

            if (!string.IsNullOrEmpty(xsdComplexType.Annotation))
            {
                complexTypeElement.Add(new XElement(
                    _xs + "annotation",
                    new XElement(_xs + "documentation", xsdComplexType.Annotation)
                    ));
            }

            var sequenceElement = BuildSequence(type);
            AddProperties(type, sequenceElement);
            complexTypeElement.Add(sequenceElement);
            return complexTypeElement;
        }

        private XElement BuildSequence(Type type)
        {
            var sequence = new XElement(_xs + "sequence");
            return sequence;
        }

        private XsdFile BuildSummary(XsdSchema defaultSchema, XElement[] defaultImports)
        {
            XsdFile xsdFile = new XsdFile();
            xsdFile.Schema = GetSchema(defaultSchema, defaultSchema.PackageId ?? _assembly.FullName);
            foreach (var import in defaultImports)
            {
                xsdFile.Imports.Add(import);
            }
            //include所有其它自动生成的xsd文件
            foreach (var item in _xsdFiles)
            {
                xsdFile.Imports.Add(new XElement(
                    _xs + "include",
                    new XAttribute("schemaLocation", item.Key + ".xsd")
                    ));
            }
            //添加dll中所有定义的element
            foreach (var item in _elements)
            {
                xsdFile.Elements.Add(item);
            }
            return xsdFile;
        }

        private void AddProperties(Type type, XElement sequenceElement)
        {
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var typeName = Common.GetXsdTypeName(propertyInfo.PropertyType);
                var propertyElement = new XElement(
                    _xs + "element",
                    new XAttribute("name", propertyInfo.Name),
                    new XAttribute("type", typeName)
                    );

                var xsdElement = XsdElement.Get(propertyInfo);
                if (xsdElement != null)
                {
                    if (!string.IsNullOrEmpty(xsdElement.MinOccurs))
                        propertyElement.SetAttributeValue("minOccurs", xsdElement.MinOccurs);
                    if (!string.IsNullOrEmpty(xsdElement.MaxOccurs))
                        propertyElement.SetAttributeValue("maxOccurs", xsdElement.MaxOccurs);

                    if (!string.IsNullOrEmpty(xsdElement.Annotation))
                        propertyElement.Add(new XElement(
                            _xs + "annotation",
                            new XElement(
                                _xs + "documentation", xsdElement.Annotation
                                )
                            ));
                }

                //判断是否自定义类型, 添加Import
                if (!typeName.StartsWith("xs:"))
                {
                    var parentClassFileGroup = XsdComplexType.Get(type).FileGroup;
                    var propertyClassFileGroup = Common.GetFileGroup(propertyInfo.PropertyType);
                    if (parentClassFileGroup != propertyClassFileGroup)
                    {
                        string importXsd = propertyClassFileGroup + ".xsd";
                        //判断是否已经存在该Import
                        if (_xsdFiles[parentClassFileGroup].Imports.All(item => item.Attribute("schemaLocation").Value != importXsd))
                        {
                            _xsdFiles[parentClassFileGroup].Imports.Add(
                                new XElement(
                                    _xs + "include",
                                    new XAttribute("schemaLocation", importXsd)
                                    )
                                );
                        }
                    }
                }
                sequenceElement.Add(propertyElement);
            }
        }

        private XElement BuildSimpleType(Type type, out string fileGroup)
        {
            var xsdSimpleType = XsdSimpleType.Get(type);
            //添加XSD文件
            fileGroup = xsdSimpleType.FileGroup;
            SetDefaultFile(fileGroup);

            var simpleTypeElement = new XElement(
                _xs + "simpleType",
                new XAttribute("name", type.Name),
                new XAttribute("final", "restriction")
                );
            var restrictionElement = new XElement(
                _xs + "restriction",
                new XAttribute("base", "xs:string")
                );

            foreach (var val in Enum.GetNames(type))
            {
                restrictionElement.Add(
                    new XElement(
                        _xs + "enumeration",
                        new XAttribute("value", val)
                        )
                    );
            }
            simpleTypeElement.Add(restrictionElement);
            return simpleTypeElement;
        }

        private void SetDefaultFile(string fileGroup)
        {
            if (!_xsdFiles.ContainsKey(fileGroup))
            {
                var xsdFile = new XsdFile();
                _xsdFiles[fileGroup] = xsdFile;
            }
        }
    }
}