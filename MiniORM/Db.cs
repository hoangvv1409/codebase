﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Linq;

namespace VinEcom.Oms.DataObject.AdoNet
{
    #region Data Layer
    // ADO.NET data access class.
    public class Db
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings[ "OrderAdmin" ].ConnectionString;
        // ** Factory pattern
        static DbProviderFactory factory = DbProviderFactories.GetFactory( "System.Data.SqlClient" );
        public string connectionString { get; set; }

        public Db( string conn = null )
        {
            if ( conn == null ) // index is 1 because 0 = localdb
                connectionString = ConfigurationManager.ConnectionStrings[ 1 ].ConnectionString;
            else
                connectionString = ConfigurationManager.ConnectionStrings[ conn ].ConnectionString;
        }


        // fast read and instantiate (i.e. make) a collection of objects

        public IEnumerable<T> Read<T>( string sql, Func<IDataReader, T> make, params object[] parms )
        {
            using ( var connection = CreateConnection() )
            {
                using ( var command = CreateCommand( sql, connection, parms ) )
                {
                    using ( var reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            yield return make( reader );
                        }
                    }
                }
            }
        }

        // use wrapper because yield cannot be in immediate try catch

        public IEnumerable<dynamic> Query( string sql, params object[] parms )
        {
            try { return QueryCore( sql, parms ); }
            catch ( Exception ex ) { throw new DbException( sql, parms, ex ); }
        }

        // fast read a list of dynamic types

        IEnumerable<dynamic> QueryCore( string sql, params object[] parms )
        {
            using ( var connection = CreateConnection() )
            {
                using ( var command = CreateCommand( sql, connection, parms ) )
                {
                    using ( var reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            yield return reader.ToExpando();
                        }
                    }
                }
            }
        }

        // executes any sql in database

        public void Execute( string sql, params object[] parms )
        {
            Update( sql, parms );
        }

        // return a scalar object

        public object Scalar( string sql, params object[] parms )
        {

            using ( var connection = CreateConnection() )
            {
                using ( var command = CreateCommand( sql, connection, parms ) )
                {
                    return command.ExecuteScalar();
                }
            }

        }

        // insert a new record

        public int Insert( string sql, params object[] parms )
        {

            using ( var connection = CreateConnection() )
            {
                using ( var command = CreateCommand( sql + ";SELECT SCOPE_IDENTITY();", connection, parms ) )
                {
                    return int.Parse( command.ExecuteScalar().ToString() );
                }
            }

        }

        // update an existing record

        public int Update( string sql, params object[] parms )
        {

            using ( var connection = CreateConnection() )
            {
                using ( var command = CreateCommand( sql, connection, parms ) )
                {
                    return command.ExecuteNonQuery();
                }
            }

        }

        // delete a record

        public int Delete( string sql, params object[] parms )
        {
            return Update( sql, parms );
        }

        #region Transaction support

        DbConnection _connection;
        DbTransaction _transaction;

        // begins a new transaction

        public void BeginTransaction()
        {
            _connection = CreateConnection();
            _transaction = _connection.BeginTransaction();
        }

        // completes an ongoing transaction

        public void EndTransaction()
        {
            _transaction.Commit();
            _connection.Close();

            _transaction.Dispose();
            _connection.Dispose();

            _transaction = null;
            _connection = null;
        }

        // insert a new record as part of a transaction

        public int TransactedInsert( string sql, params object[] parms )
        {
            try
            {
                using ( var command = CreateCommand( sql + ";SELECT SCOPE_IDENTITY();", _connection, parms ) )
                {
                    command.Transaction = _transaction;

                    return int.Parse( command.ExecuteScalar().ToString() );
                }
            }
            catch ( Exception ex ) { throw new DbException( sql, parms, ex ); }
        }

        // update a record as part of a transaction

        public int TransactedUpdate( string sql, params object[] parms )
        {
            try
            {
                using ( var command = CreateCommand( sql, _connection, parms ) )
                {
                    command.Transaction = _transaction;
                    return command.ExecuteNonQuery();
                }
            }
            catch ( Exception ex ) { throw new DbException( sql, parms, ex ); }
        }

        // delete a record as apart of a transaction

        public int TransactedDelete( string sql, params object[] parms )
        {
            return TransactedUpdate( sql, parms );
        }

        #endregion

        // creates a connection object

        DbConnection CreateConnection()
        {
            // ** Factory pattern in action

            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }

        // creates a command object

        DbCommand CreateCommand( string sql, DbConnection conn, params object[] parms )
        {
            // ** Factory pattern in action

            var command = factory.CreateCommand();
            command.Connection = conn;
            command.CommandText = sql;
            command.AddParameters( parms );
            return command;
        }

        // creates an adapter object

        DbDataAdapter CreateAdapter( DbCommand command )
        {
            // ** Factory pattern in action

            var adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            return adapter;
        }
    }

    // extension methods

    public static class DbExtentions
    {
        // adds parameters to a command object

        public static void AddParameters( this DbCommand command, object[] parms )
        {
            if ( parms != null && parms.Length > 0 )
            {

                // ** Iterator pattern

                // NOTE: processes a name/value pair at each iteration

                for ( int i = 0; i < parms.Length; i += 2 )
                {
                    string name = parms[ i ].ToString();

                    // no empty strings to the database

                    if ( parms[ i + 1 ] is string && (string)parms[ i + 1 ] == "" )
                        parms[ i + 1 ] = null;

                    // if null, set to DbNull

                    object value = parms[ i + 1 ] ?? DBNull.Value;

                    // ** Factory pattern

                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = name;
                    dbParameter.Value = value;

                    command.Parameters.Add( dbParameter );
                }
            }
        }

        // iterate over fields in datareader and returns an expando object

        public static dynamic ToExpando( this IDataReader reader )
        {
            var dictionary = new ExpandoObject() as IDictionary<string, object>;
            for ( int i = 0; i < reader.FieldCount; i++ )
                dictionary.Add( reader.GetName( i ), reader[ i ] == DBNull.Value ? null : reader[ i ] );

            return dictionary as ExpandoObject;
        }
    }

    // custom exception which holds Db execution context

    public class DbException : Exception
    {
        public DbException()
        {
        }
        public DbException( string message )
            : base( "In Db: " + message )
        {
        }

        public DbException( string message, Exception innerException )
            : base( "In Db: " + message, innerException )
        {
        }

        public DbException( string sql, object[] parms, Exception innerException )
            : base( "In Db: " + string.Format( "Sql: {0}  Parms: {1}", ( sql ?? "--" ),
                    ( parms != null ? string.Join( ",", Array.ConvertAll<object, string>( parms, o => ( o ?? "null" ).ToString() ) ) : "--" ) ),
            innerException )
        {
        }
    }

    #endregion

    #region Domain Layer

    // entity class. base class to all domain objects

    public partial class Entity<T> where T : Entity<T>, new()
    {
        protected static string tableName { get; set; }
        protected static string keyName { get; set; }
        protected static Db db { get; set; }
        protected static bool audit { get; set; }

        static Dictionary<string, PropertyInfo> props { get; set; }
        static Dictionary<string, SchemaMap> map { get; set; }

        static string sqlSelect { get; set; }
        static string sqlInsert { get; set; }
        static string sqlUpdate { get; set; }
        static string sqlDelete { get; set; }
        static string sqlPaged { get; set; }

        #region Static initialization

        static Entity()
        {
            Init();
        }

        // this allows custom db, table, and key to be specified at startup

        public static void Init( Db db = null, string table = null, string key = null )
        {
            InitDb( db, table, key );
            InitMap();

            InitSelect();
            InitInsert();
            InitUpdate();
            InitDelete();
            InitPaged();
        }

        // sets db, table and primary key names

        static void InitDb( Db _db, string table, string key )
        {
            //db = _db ?? new ArtDb();
            //tableName = table ?? typeof(T).Name;
            //keyName = key ?? "Id";
        }

        // creates a column-to-property map and their default values

        static void InitMap()
        {
            props = typeof( T ).GetProperties().ToDictionary( p => p.Name );
            map = new Dictionary<string, SchemaMap>();

            foreach ( dynamic column in Columns )
            {
                if ( props.ContainsKey( column.COLUMN_NAME ) )
                {
                    map.Add( column.COLUMN_NAME, new SchemaMap
                    {
                        Prop = props[ column.COLUMN_NAME ],
                        Default = Default( column.COLUMN_DEFAULT )
                    } );
                }
            }
        }

        static string[] auditColumns = new[] { "CreatedOn", "CreatedBy", "ChangedOn", "ChangedBy" };

        // retrieves column schema data from database

        static IEnumerable<dynamic> Columns
        {
            get
            {
                // get all columns for table

                string sql = @"SELECT COLUMN_NAME, COLUMN_DEFAULT, DATA_TYPE
                                 FROM INFORMATION_SCHEMA.COLUMNS
                                WHERE TABLE_NAME = @0";

                var columns = db.Query( sql, tableName );
                var noAuditColumns = columns.Where( c => !auditColumns.Contains( (string)c.COLUMN_NAME ) );

                // with all audit columns present auditing will be supported

                audit = columns.Count() - noAuditColumns.Count() == auditColumns.Count();
                if ( audit )
                    return noAuditColumns;

                return columns;
            }
        }

        // returns clean column default value

        static object Default( string value )
        {
            if ( value.IsNullOrEmpty() )
                return null;
            else if ( value.Contains( "getdate()" ) )
                return "getdate()";
            else if ( value.Contains( "newid()" ) )
                return Guid.NewGuid();

            string val = value.Replace( "(N'", "" ).Replace( "(", "" ).Replace( ")", "" ).Replace( "'", "" );

            // cast default values

            int i;
            if ( int.TryParse( val, out i ) )
                return i;
            double d;
            if ( double.TryParse( val, out d ) )
                return d;

            return val;
        }

        #endregion

        #region Constructors

        // default contructor

        public Entity() { }

        // creates new entity with optional default values

        public Entity( bool defaults = false )
        {
            if ( defaults )
            {
                foreach ( var item in map.Values )
                    item.Prop.SetValue( this, item.Default.OrNow(), null );
            }
        }

        #endregion

        // retrieves a single object by id

        public T Single( int? id )
        {
            string sql = CreateSelect( keyName + " = @0 " );
            return db.Read( sql, Make, id ).FirstOrDefault();
        }

        // retrieves a single object with a where clause

        public T Single( string where = null, params object[] parms )
        {
            string sql = CreateSelect( where );
            return db.Read( sql, Make, parms ).FirstOrDefault();
        }

        // retrieves a list of objects by their ids

        public IEnumerable<T> All( string ids )
        {
            string sql = CreateSelect( keyName + " IN (" + ids + ") " );
            return db.Read( sql, Make, null );
        }

        // retrieves a list of objects given several criteria

        public IEnumerable<T> All( string where = null, string orderBy = null, int top = 0, params object[] parms )
        {
            string sql = CreateSelect( where, orderBy, top );
            return db.Read( sql, Make, parms );
        }

        // retrieves a paged list of objects given several criteia

        public IEnumerable<T> Paged( out int totalRows, string where = null, string orderBy = null, int page = 0, int pageSize = 20, params object[] parms )
        {
            totalRows = Count( where, parms );

            string sql = CreatePaged( where, orderBy, page, pageSize );
            return db.Read( sql, Make, parms );
        }

        // retrieves any scalar value by criteria

        public virtual object Scalar( string operation, string column = null, string where = null, params object[] parms )
        {
            string sql = CreateScalar( operation, column, where );
            return db.Scalar( sql, parms );
        }

        // retrieves a scalar count value by criteria

        public virtual int Count( string where = null, params object[] parms )
        {
            string sql = CreateScalar( "COUNT", keyName, where );
            return (int)db.Scalar( sql, parms );
        }

        // retrieves a scalar max value by criteria

        public virtual object Max( string column = null, string where = null, params object[] parms )
        {
            string sql = CreateScalar( "MAX", column ?? keyName, where );
            object o = db.Scalar( sql, parms );
            return o is DBNull ? null : o;
        }

        // retrieves a scalar min value by criteria

        public virtual object Min( string column = null, string where = null, params object[] parms )
        {
            string sql = CreateScalar( "MIN", column ?? keyName, where );
            object o = db.Scalar( sql, parms );
            return o is DBNull ? null : o;
        }

        // retrieves a scalar sum value by criteria

        public virtual object Sum( string column = null, string where = null, params object[] parms )
        {
            string sql = CreateScalar( "SUM", column ?? keyName, where );
            object o = db.Scalar( sql, parms );
            return o is DBNull ? null : o;
        }

        // iterates over data reader fields and populates an entity instance

        static Func<IDataReader, T> Make = reader =>
        {
            T t = new T();

            for ( int i = 0; i < reader.FieldCount; i++ )
            {
                string key = reader.GetName( i );
                object val = reader[ i ] == DBNull.Value ? null : reader[ i ];
                map[ key ].Prop.SetValue( t, val, null );
            }

            return t;
        };

        // iterates over object properties and prepares these as sql parameters

        protected object[] Take()
        {
            var objects = new List<object>();

            foreach ( var item in map.Values )
            {
                objects.Add( "@" + item.Prop.Name );
                objects.Add( item.Prop.GetValue( this, null ) );
            }

            if ( audit )
            {
                var dt = DateTime.Now;
                var userId = TryGetUserId();

                objects.Add( "@CreatedOn" );
                objects.Add( dt );
                objects.Add( "@CreatedBy" );
                objects.Add( userId );
                objects.Add( "@ChangedOn" );
                objects.Add( dt );
                objects.Add( "@ChangedBy" );
                objects.Add( userId );
            }

            return objects.ToArray();
        }

        // try getting user id from thread

        protected virtual int? TryGetUserId()
        {
            if ( !Thread.CurrentPrincipal.Identity.IsAuthenticated )
                return null;

            int? userId = null;

            try
            {
                // principal is of type CustomPrincipal as defined in Web project

                dynamic principal = Thread.CurrentPrincipal;

                userId = principal.UserId;
                if ( userId != null )
                    return userId;
            }
            catch { /* no action */}

            return userId;
        }

        // retrieves a list of dynamic objects using an arbitrary sql query

        public IEnumerable<T> Query( string sql, params object[] parms )
        {
            var items = db.Query( sql, parms );
            foreach ( var item in items )
            {
                yield return ToType( item );
            }
        }

        // maps an expando object to a typed entity instance

        static Func<ExpandoObject, T> ToType = expando =>
        {
            T t = new T();

            var dictionary = expando as IDictionary<string, object>;
            foreach ( var key in dictionary.Keys )
            {
                if ( map.ContainsKey( key ) )
                    map[ key ].Prop.SetValue( t, dictionary[ key ], null );
            }
            return t;
        };

        // executes any sql in database

        public void Execute( string sql, params object[] parms )
        {
            db.Execute( sql, parms );
        }

        // helper: creates a sql select statement

        string CreateSelect( string where, string orderBy = null, int top = 0 )
        {
            string t = top > 0 ? "TOP " + top : null;
            string w = where.IsNullOrEmpty() ? null : "WHERE " + where;
            string o = "ORDER BY " + ( orderBy.IsNullOrEmpty() ? keyName : orderBy );

            return string.Format( sqlSelect, t, w, o );
        }

        // helper: creates a sql select statement for pagination

        string CreatePaged( string where, string orderBy, int page, int pageSize )
        {
            string t = "TOP " + pageSize;
            string w = where.IsNullOrEmpty() ? null : "WHERE " + where;
            string o = orderBy.IsNullOrEmpty() ? keyName : orderBy;
            string r = "WHERE Row > " + ( ( page - 1 ) * pageSize );

            return string.Format( sqlPaged, t, w, o, r );
        }

        // helper: creates a scalar sql select statement

        string CreateScalar( string operation, string column, string where = null )
        {
            string op = operation.ToUpper();
            string c = column ?? keyName;
            string t = tableName.Bracket();
            string w = where.IsNullOrEmpty() ? null : "WHERE " + where;

            return string.Format( "SELECT {0}({1}) FROM {2} {3}", op, c, t, w );
        }

        // indexer. this provides easy access to properties

        object this[ string name ]
        {
            get { return map[ name ].Prop.GetValue( this, null ); }
            set { map[ name ].Prop.SetValue( this, value, null ); }
        }

        #region Initialize Sql Statements

        // builds a sql select template string

        static void InitSelect()
        {
            var cols = new StringBuilder();

            foreach ( var key in map.Keys )
                cols.AppendFormat( "{0}, ", key );

            // {{0}} is placeholders for TOP, {{1}} and {{2}} for WHERE, ORDER BY.
            string sql = "SELECT {{0}} {0} FROM {1} {{1}} {{2}}";
            sqlSelect = string.Format( sql, cols.TrimEnd(), tableName.Bracket() );
        }

        // builds a sql select template string for pagination

        static void InitPaged()
        {
            var cols = new StringBuilder();
            foreach ( var key in map.Keys )
                cols.AppendFormat( "{0}, ", key );

            // {{0}} is for TOP, {{1}} and {{2}} for WHERE, ORDER BY, {{3}} for Row WHERE.
            string sql = "SELECT {{0}} {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {{2}}) AS Row, {0} FROM {1} {{1}} ) AS Paged {{3}}";
            sqlPaged = string.Format( sql, cols.TrimEnd(), tableName.Bracket() );
        }

        // builds a sql insert template string

        static void InitInsert()
        {
            var cols = new StringBuilder();
            var vals = new StringBuilder();

            foreach ( var key in PropsWithoutPrimaryKey )
            {
                cols.AppendFormat( "{0}, ", key );
                vals.AppendFormat( "@{0}, ", key );
            }

            string sql = "INSERT INTO {0} ({1}) VALUES ({2})";
            sqlInsert = string.Format( sql, tableName.Bracket(), cols.TrimEnd(), vals.TrimEnd() );
        }

        // builds a sql update template string

        static void InitUpdate()
        {
            var sets = new StringBuilder();

            foreach ( var key in PropsWithoutPrimaryKey )
            {
                // don't include 'create' audit fields
                if ( audit && ( key == "CreatedOn" || key == "CreatedBy" ) )
                    continue;

                sets.AppendFormat( "{0}=@{1}, ", key, key );
            }

            string sql = "UPDATE {0} SET {1} WHERE {2} = @{3}";
            sqlUpdate = string.Format( sql, tableName.Bracket(), sets.TrimEnd(), keyName, keyName );
        }

        // builds a sql delete template string

        static void InitDelete()
        {
            string sql = "DELETE FROM {0} WHERE {1} = @{2}";
            sqlDelete = string.Format( sql, tableName.Bracket(), keyName, keyName );
        }

        // returns all list of properties except primary key

        static IEnumerable<string> PropsWithoutPrimaryKey
        {
            get
            {
                foreach ( var key in map.Keys )
                    if ( key != keyName )
                        yield return key;

                if ( audit )
                    foreach ( var key in auditColumns )
                        yield return key;
            }
        }

        #endregion

        // partial methods that allow custom coding (for all entities) before and after all db operations

        partial void OnInsertingAll( ref string sql );
        partial void OnInsertedAll();
        partial void OnUpdatingAll( ref string sql );
        partial void OnUpdatedAll();
        partial void OnDeletingAll( ref string sql );
        partial void OnDeletedAll();

        // virtual methods that allow custom coding (by entity type) before and after all db operations

        protected virtual void OnInserting( ref string sql ) { }
        protected virtual void OnInserted() { }
        protected virtual void OnUpdating( ref string sql ) { }
        protected virtual void OnUpdated() { }
        protected virtual void OnDeleting( ref string sql ) { }
        protected virtual void OnDeleted() { }

        // inserts current entity instance

        public virtual void Insert()
        {
            string sql = sqlInsert;

            OnInsertingAll( ref sql );
            OnInserting( ref sql );
            this[ keyName ] = db.Insert( sql, Take() );
            OnInserted();
            OnInsertedAll();
        }

        // updates current entity instance

        public virtual void Update()
        {
            string sql = sqlUpdate;

            OnUpdatingAll( ref sql );
            OnUpdating( ref sql );
            db.Update( sql, Take() );
            OnUpdated();
            OnUpdatedAll();
        }

        // deletes current entity instance

        public virtual void Delete()
        {
            string sql = sqlDelete;

            OnDeletingAll( ref sql );
            OnDeleting( ref sql );
            db.Delete( sql, Take() );
            OnDeleted();
            OnDeletedAll();
        }

        #region Validation

        protected virtual void Validate() { }

        // executes validations and returns a boolean result

        public bool IsValid
        {
            get
            {
                Errors.Clear();
                Validate();
                return Errors.Count == 0;
            }
        }

        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        #endregion

        #region Transacted actions

        // inserts current entity instance as part of an ongoing transaction

        public virtual void TransactedInsert( Db db )
        {
            string sql = sqlInsert;

            OnInsertingAll( ref sql );
            OnInserting( ref sql );
            this[ keyName ] = db.TransactedInsert( sql, Take() );
            OnInserted();
            OnInsertedAll();
        }

        // updates current entity instance as part of an ongoing transaction

        public virtual void TransactedUpdate( Db db )
        {
            string sql = sqlUpdate;

            OnUpdatingAll( ref sql );
            OnUpdating( ref sql );
            db.TransactedUpdate( sql, Take() );
            OnUpdated();
            OnUpdatedAll();
        }

        // deletes current entity instance as part of an ongoing transaction

        public virtual void TransactedDelete( Db db )
        {
            string sql = sqlDelete;

            OnDeletingAll( ref sql );
            OnDeleting( ref sql );
            db.TransactedDelete( sql, Take() );
            OnDeleted();
            OnDeletedAll();
        }

        #endregion

        // holds an object property and its default value

        class SchemaMap
        {
            public PropertyInfo Prop { get; set; }
            public object Default { get; set; }
        }
    }

    // entity extension

    static class EntityExtensions
    {
        public static string TrimEnd( this StringBuilder sb )
        {
            return sb.ToString().TrimEnd( new char[] { ',', ' ', '|' } );
        }
        public static string Bracket( this string item )
        {
            return "[" + item + "]";
        }
        public static bool IsNullOrEmpty( this string s )
        {
            return string.IsNullOrEmpty( s );
        }
        public static object OrNow( this object value )
        {
            if ( value != null && value.ToString() == "getdate()" )
                return DateTime.Now;
            return value;
        }
    }

    #endregion

    #region Unit Of Work Pattern

    // manages local transactions

    public partial class UnitOfWork : IDisposable
    {
        protected Db db { get; set; }

        public UnitOfWork( Db db )
        {
            this.db = db;
            db.BeginTransaction();
        }

        public virtual void Insert<T>( T entity ) where T : Entity<T>, new()
        {
            entity.TransactedInsert( db );
        }
        public virtual void Update<T>( T entity ) where T : Entity<T>, new()
        {
            entity.TransactedUpdate( db );
        }
        public virtual void Delete<T>( T entity ) where T : Entity<T>, new()
        {
            entity.TransactedDelete( db );
        }

        public virtual void Dispose()
        {
            db.EndTransaction();
        }
    }

    // manages distributed transactions.

    public partial class UnitOfWorkDistributed : IDisposable
    {
        protected TransactionScope scope;

        public UnitOfWorkDistributed()
        {
            // TransactionScope requires that MSDTC is running.
            // first ensure that MSDTC runs, then uncomment line below

            scope = new TransactionScope();
        }

        public virtual void Complete()
        {
            if ( scope != null )
                scope.Complete();
            scope = null;
        }

        public virtual void Dispose()
        {
            Complete();
        }
    }

    #endregion
}
