using System.Collections.Generic;
using System.Xml.Linq;

namespace XsdGen
{
    public class XsdFile
    {
        public XsdFile()
        {
            Imports = new List<XElement>();
            Elements = new List<XElement>();
        }

        public XElement Schema { get; set; }

        /// <summary>
        ///     <para>Assembly级别的Import</para>
        ///     <para>自定义复合对象的Import</para>
        /// </summary>
        public List<XElement> Imports { get; set; }

        /// <summary>
        ///     <para>自定义Class类型</para>
        ///     <para>自定义枚举类型</para>
        /// </summary>
        public List<XElement> Elements { get; set; }

        public XElement ToXML()
        {
            foreach (var import in Imports)
            {
                Schema.Add(import);
            }
            foreach (var element in Elements)
            {
                Schema.Add(element);
            }
            return Schema;
        }
    }
}