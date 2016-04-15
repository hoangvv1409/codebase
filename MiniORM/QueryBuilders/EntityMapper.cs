using MiniOrm.ORM.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DapperOrm
{
    public class EntityMapper<T> where T : class, new()
    {
        #region private fields

        private static Dictionary<string, PropertyInfo> entitiesPropertyInfo = new Dictionary<string, PropertyInfo>();
        private Dictionary<object, T> lookupObject;
        private Func<T, object> rowKey;
        private Dictionary<string, Dictionary<object, bool>> propertiesObject;
        private List<PropertyMapper> mapperProperties;

        #endregion

        #region Constructors
        static EntityMapper()
        {
            Type entityType = typeof( T );
            var properties = entityType.GetProperties().Where( p => !p.PropertyType.IsSimpleType() );
            foreach ( var p in properties )
            {
                entitiesPropertyInfo.Add( p.Name, p );
            }
        }

        public EntityMapper( Func<T, object> rowKey )
        {
            this.lookupObject = new Dictionary<object, T>();
            this.propertiesObject = new Dictionary<string, Dictionary<object, bool>>();
            this.rowKey = rowKey;
            this.mapperProperties = new List<PropertyMapper>();
        }


        public EntityMapper( Func<T, object> rowKey, List<PropertyMapper> mapperProperties )
            : this( rowKey )
        {
            this.mapperProperties = mapperProperties;
        }

        #endregion

        #region Base mappers
        public void AddMapper<TProperty>( PropertyMapper<TProperty> mapper )
        {
            if ( !this.mapperProperties.Exists( p => p.PropertyName == mapper.PropertyName ) )
                this.mapperProperties.Add( mapper );
        }

        public bool AddRowIfNotExist( T t, out T current )
        {
            object key = this.rowKey( t );
            if ( !this.lookupObject.TryGetValue( key, out current ) )
            {
                this.lookupObject.Add( key, t );
                current = t;
                return false;
            }
            return true;
        }

        public void Map<TField>( T parrentRow, string propertyName, TField fieldValue, Func<TField, object> propertyKey )
        {
            Dictionary<object, bool> propertyObject = null;
            if ( !this.propertiesObject.TryGetValue( propertyName, out propertyObject ) )
            {
                propertyObject = new Dictionary<object, bool>();
                this.propertiesObject.Add( propertyName, propertyObject );
            }

            object fieldKey = propertyKey( fieldValue );
            if ( !propertyObject.ContainsKey( fieldKey ) )
            {
                propertyObject.Add( fieldKey, true );
                SetComplexProperty( parrentRow, propertyName, fieldValue );
            }
        }
        #endregion

        #region Mapper

        public void MapProperty<T1>( T currentRow, T1 t1 )
        {
            PropertyMapper<T1> map = (PropertyMapper<T1>)this.mapperProperties[ 0 ];
            this.Map<T1>( currentRow, map.PropertyName, t1, map.Key );
        }

        public void MapProperty<T1, T2>( T currentRow, T1 t1, T2 t2 )
        {
            MapProperty( currentRow, t1 );

            PropertyMapper<T2> map = (PropertyMapper<T2>)this.mapperProperties[ 1 ];
            this.Map<T2>( currentRow, map.PropertyName, t2, map.Key );
        }

        public void MapProperty<T1, T2, T3>( T currentRow, T1 t1, T2 t2, T3 t3 )
        {
            MapProperty( currentRow, t1, t2 );

            PropertyMapper<T3> map = (PropertyMapper<T3>)this.mapperProperties[ 2 ];
            this.Map<T3>( currentRow, map.PropertyName, t3, map.Key );
        }

        public void MapProperty<T1, T2, T3, T4>( T currentRow, T1 t1, T2 t2, T3 t3, T4 t4 )
        {
            MapProperty( currentRow, t1, t2, t3 );

            PropertyMapper<T4> map = (PropertyMapper<T4>)this.mapperProperties[ 3 ];
            this.Map<T4>( currentRow, map.PropertyName, t4, map.Key );
        }

        public void MapProperty<T1, T2, T3, T4, T5>( T currentRow, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 )
        {
            MapProperty( currentRow, t1, t2, t3, t4 );

            PropertyMapper<T5> map = (PropertyMapper<T5>)this.mapperProperties[ 4 ];
            this.Map<T5>( currentRow, map.PropertyName, t5, map.Key );
        }

        public void MapProperty<T1, T2, T3, T4, T5, T6>( T currentRow, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 )
        {
            MapProperty( currentRow, t1, t2, t3, t4, t5 );

            PropertyMapper<T6> map = (PropertyMapper<T6>)this.mapperProperties[ 5 ];
            this.Map<T6>( currentRow, map.PropertyName, t6, map.Key );
        }

        public void MapProperty<T1, T2, T3, T4, T5, T6, T7>( T currentRow, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 )
        {
            MapProperty( currentRow, t1, t2, t3, t4, t5, t6 );

            PropertyMapper<T7> map = (PropertyMapper<T7>)this.mapperProperties[ 6 ];
            this.Map<T7>( currentRow, map.PropertyName, t7, map.Key );
        }

        #endregion

        #region set property values
        private void SetComplexProperty( T entity, string propertyName, object value )
        {
            PropertyInfo proInfo = entitiesPropertyInfo[ propertyName ];
            if ( !proInfo.PropertyType.IsCollection() )
            {
                proInfo.SetValue( entity, value );
                return;
            }

            // neu la collection thi
            var colletionValue = proInfo.GetValue( entity );
            if ( colletionValue == null )
            {
                colletionValue = Activator.CreateInstance( proInfo.PropertyType );
                AddToCollection( colletionValue, value, proInfo );

                proInfo.SetValue( entity, colletionValue );
                return;
            }

            AddToCollection( colletionValue, value, proInfo );
            return;
        }

        private void AddToCollection( object collection, object value, PropertyInfo proInfo )
        {
            MethodInfo methodInfo = proInfo.PropertyType.GetMethod( "Add" );
            object[] values = new object[] { value };

            methodInfo.Invoke( collection, values );
        }
        #endregion
    }
}
