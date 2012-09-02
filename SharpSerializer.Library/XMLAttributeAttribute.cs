using System;
using System.Collections.Generic;
using System.Text;

namespace Polenter.Serialization
{
    /// <summary>
    /// attribute to specify a property to serialize to a named XML attribute instead of a child node
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class XMLAttributeAttribute : System.Attribute
    {
        public string AttributeName { get; set; }

        /// <summary>
        /// constructs .NET attribute that specifies XML attribute
        /// </summary>
        /// <param name="attrib">XML attribute name to serialize to</param>
        public XMLAttributeAttribute(string attrib)
        {
            AttributeName = attrib;
        }
    }
}
