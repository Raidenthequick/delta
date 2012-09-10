using System;
using System.Collections.Generic;
using System.Text;

namespace Polenter.Serialization.Core
{
    /// <summary>
    /// to control what types register as "simple"
    /// </summary>
    public class SimpleTypes
    {
        List<Type> _types;

        public SimpleTypes(Type[] types)
        {
            _types = new List<Type>(types);
        }

        /// <summary>
        /// is the type simple?
        /// </summary>
        /// <param name="type"></param>
        /// <returns>true for simple</returns>
        public bool IsSimple(Type type)
        {
            return _types.Contains(type);
        }
    }
}
