using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperOrm
{
    public static class TypeExtension
    {
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || type.Equals(typeof(string)) || (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        public static bool IsCollection(this Type type)
        {
            var colInterface = type.GetInterfaces().Where(f => f.IsGenericType && typeof(ICollection<>) == f.GetGenericTypeDefinition()).FirstOrDefault();
            return colInterface != null;
        }

        public static string GetTableAttribute(this Type type )
        {
            TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute( type, typeof( TableAttribute ) );
            if ( tableAttribute != null )
            {
                return tableAttribute.Name;
            }
            throw new Exception( "Missing entity table attribute" );
        }
    }
}
