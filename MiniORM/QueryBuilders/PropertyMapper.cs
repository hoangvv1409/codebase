using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperOrm
{
    public class PropertyMapper
    {
        public PropertyMapper( string propertyName )
        {
            this.PropertyName = propertyName;
        }
        public string PropertyName { get; private set; }
    }

    public class PropertyMapper<T> : PropertyMapper
    {
        public PropertyMapper( string propertyName, Func<T, Object> Key )
            : base( propertyName )
        {
            this.Key = Key;
        }

        public Func<T, Object> Key { get; private set; }
    }
}
