using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperOrm
{
    public class NavigationField
    {
        public string WithEntityProperty { get; protected set; }
        public string Table { get; protected set; }
        public string Relation { get; protected set; }
        public string Select { get; protected set; }
        public string SplitOn { get; protected set; }
    }
    
    public class NavigationField<T> : NavigationField
    {
        public NavigationField()
        { }

        public NavigationField( string table, string splitOn, Func<T, object> key, string navigationKey, string relation )
        {
            this.Table = table;
            this.SplitOn = splitOn;
            this.Select = string.Format( "{0}.*", table );
            this.Key = key;
            this.Relation = string.Format( "inner join {0} on {1}.[{2}] = {3}", table, table, navigationKey, relation );
        }

        public NavigationField( string withEntityProperty, string fromTable, string fromTableField, string toTable, string toTableField, Func<T, object> Key )
        {
            this.SplitOn = withEntityProperty;
            this.Table = toTable;
            this.Select = string.Format( "{0}.*", this.Table );
            this.Relation = string.Format( "inner join {0} on {1}.[{2}] = {3}.[{4}]", this.Table, this.Table, toTableField, fromTable, fromTableField );
            this.Key = Key;
        }

        public Func<T, object> Key { get; protected set; }
    }

    public class NavigationField<FromT, ToT> : NavigationField
    {
        public NavigationField( string withEntityProperty, string fromTableField, string toTableField, Func<ToT, object> FromTableKey )
        {
            this.WithEntityProperty = withEntityProperty;
            this.SplitOn = withEntityProperty;
            this.Table = typeof( ToT ).GetTableAttribute();
            this.Select = string.Format( "{0}.*", this.Table );
            string fromTable = typeof( FromT ).GetTableAttribute();
            this.Relation = string.Format( "inner join {0} on {1}.[{2}] = {3}.[{4}]", this.Table, this.Table, toTableField, fromTable, fromTableField );
            this.FromTableKey = FromTableKey;
        }

        public Func<ToT, object> FromTableKey { get; protected set; }
    }
}
