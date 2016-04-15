using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlapperMapping;
using MiniOrm.ORM.QueryBuilders;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;

namespace DapperOrm
{
    public class QueryBuilder<T> where T : class, new()
    {
        private string table;
        private string keyName;
        private Func<T, object> key;
        private ExpandoObject parameters;

        private List<Predicate> predicates;
        private EntityMapper<T> mapper;
        private string splitOn;

        private Dictionary<string, NavigationField> navigationFields;
        private Dictionary<string, NavigationField> chosenNavigationFields;

        private Dictionary<string, Field> simpleFields;
        private Dictionary<string, Field> chosenSimpleFields;
        private List<SplitField> splitFields;
        private Dictionary<string, bool> includedFields;

        public QueryBuilder()
        {
            this.table = typeof( T ).GetTableAttribute();
            splitOn = string.Empty;

            predicates = new List<Predicate>();
            parameters = new ExpandoObject();

            includedFields = new Dictionary<string, bool>();

            navigationFields = new Dictionary<string, NavigationField>();
            chosenNavigationFields = new Dictionary<string, NavigationField>();
            splitFields = new List<SplitField>();

            simpleFields = new Dictionary<string, Field>();
            chosenSimpleFields = new Dictionary<string, Field>();
            InitSimpleField();
        }

        public void HasKey( string keyName, Func<T, object> key )
        {
            this.key = key;
            this.keyName = keyName;
            mapper = new EntityMapper<T>( this.key );
        }

        #region navigation and split on

        #region navigation
        public void Navigate<ToT1>( NavigationField<T, ToT1> nav1 )
        {
            this.navigationFields.Add( nav1.WithEntityProperty, nav1 );
            this.mapper.AddMapper<ToT1>( new PropertyMapper<ToT1>( nav1.WithEntityProperty, nav1.FromTableKey ) );
            splitFields.Add( new SplitField() { Keyword = nav1.SplitOn, Select = string.Empty } );
        }

        public void Navigate<ToT1, ToT2>( NavigationField<T, ToT1> nav1, NavigationField<T, ToT2> nav2 )
        {
            this.Navigate( nav1 );

            this.navigationFields.Add( nav2.WithEntityProperty, nav2 );
            this.mapper.AddMapper<ToT2>( new PropertyMapper<ToT2>( nav2.WithEntityProperty, nav2.FromTableKey ) );
            splitFields.Add( new SplitField() { Keyword = nav2.SplitOn, Select = string.Empty } );
        }

        public void Navigate<ToT1, ToT2, ToT3>( NavigationField<T, ToT1> nav1, NavigationField<T, ToT2> nav2, NavigationField<T, ToT3> nav3 )
        {
            this.Navigate( nav1, nav2 );

            this.navigationFields.Add( nav3.WithEntityProperty, nav3 );
            this.mapper.AddMapper<ToT3>( new PropertyMapper<ToT3>( nav3.WithEntityProperty, nav3.FromTableKey ) );
            splitFields.Add( new SplitField() { Keyword = nav3.SplitOn, Select = string.Empty } );
        }

        public void Navigate<ToT1, ToT2, ToT3, ToT4>( NavigationField<T, ToT1> nav1, NavigationField<T, ToT2> nav2, NavigationField<T, ToT3> nav3, NavigationField<T, ToT4> nav4 )
        {
            this.Navigate( nav1, nav2, nav3 );

            this.navigationFields.Add( nav4.WithEntityProperty, nav4 );
            this.mapper.AddMapper<ToT4>( new PropertyMapper<ToT4>( nav4.WithEntityProperty, nav4.FromTableKey ) );
            splitFields.Add( new SplitField() { Keyword = nav4.SplitOn, Select = string.Empty } );
        }

        public void Navigate<ToT1, ToT2, ToT3, ToT4, ToT5>( NavigationField<T, ToT1> nav1, NavigationField<T, ToT2> nav2, NavigationField<T, ToT3> nav3, NavigationField<T, ToT4> nav4, NavigationField<T, ToT5> nav5 )
        {
            this.Navigate( nav1, nav2, nav3, nav4 );

            this.navigationFields.Add( nav5.WithEntityProperty, nav5 );
            this.mapper.AddMapper<ToT5>( new PropertyMapper<ToT5>( nav5.WithEntityProperty, nav5.FromTableKey ) );
            splitFields.Add( new SplitField() { Keyword = nav5.SplitOn, Select = string.Empty } );
        }

        public void Navigate<ToT1, ToT2, ToT3, ToT4, ToT5, ToT6>( NavigationField<T, ToT1> nav1, NavigationField<T, ToT2> nav2, NavigationField<T, ToT3> nav3, NavigationField<T, ToT4> nav4, NavigationField<T, ToT5> nav5, NavigationField<T, ToT6> nav6 )
        {
            this.Navigate( nav1, nav2, nav3, nav4, nav5 );

            this.navigationFields.Add( nav6.WithEntityProperty, nav6 );
            this.mapper.AddMapper<ToT6>( new PropertyMapper<ToT6>( nav6.WithEntityProperty, nav6.FromTableKey ) );
            splitFields.Add( new SplitField() { Keyword = nav6.SplitOn, Select = string.Empty } );
        }

        public void Navigate<ToT1, ToT2, ToT3, ToT4, ToT5, ToT6, ToT7>( NavigationField<T, ToT1> nav1, NavigationField<T, ToT2> nav2, NavigationField<T, ToT3> nav3, NavigationField<T, ToT4> nav4, NavigationField<T, ToT5> nav5, NavigationField<T, ToT6> nav6, NavigationField<T, ToT7> nav7 )
        {
            this.Navigate( nav1, nav2, nav3, nav4, nav5, nav6 );

            this.navigationFields.Add( nav6.WithEntityProperty, nav7 );
            this.mapper.AddMapper<ToT7>( new PropertyMapper<ToT7>( nav7.WithEntityProperty, nav7.FromTableKey ) );
            splitFields.Add( new SplitField() { Keyword = nav7.SplitOn, Select = string.Empty } );
        }
        #endregion

        #region Split

        public T Split<T1>( T t, T1 t1 )
        {
            T currentRow = null;
            bool found = this.mapper.AddRowIfNotExist( t, out currentRow );
            this.mapper.MapProperty( currentRow, t1 );

            return found ? null : currentRow;
        }

        public T Split<T1, T2>( T t, T1 t1, T2 t2 )
        {
            T currentRow = null;
            bool found = this.mapper.AddRowIfNotExist( t, out currentRow );
            this.mapper.MapProperty( currentRow, t1, t2 );

            return found ? null : currentRow;
        }

        public T Split<T1, T2, T3>( T t, T1 t1, T2 t2, T3 t3 )
        {
            T currentRow = null;
            bool found = this.mapper.AddRowIfNotExist( t, out currentRow );
            this.mapper.MapProperty( currentRow, t1, t2, t3 );

            return found ? null : currentRow;
        }

        public T Split<T1, T2, T3, T4>( T t, T1 t1, T2 t2, T3 t3, T4 t4 )
        {
            T currentRow = null;
            bool found = this.mapper.AddRowIfNotExist( t, out currentRow );
            this.mapper.MapProperty( currentRow, t1, t2, t3, t4 );

            return found ? null : currentRow;
        }
        public T Split<T1, T2, T3, T4, T5>( T t, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 )
        {
            T currentRow = null;
            bool found = this.mapper.AddRowIfNotExist( t, out currentRow );
            this.mapper.MapProperty( currentRow, t1, t2, t3, t4, t5 );

            return found ? null : currentRow;
        }

        public T Split<T1, T2, T3, T4, T5, T6>( T t, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6 )
        {
            T currentRow = null;
            bool found = this.mapper.AddRowIfNotExist( t, out currentRow );
            this.mapper.MapProperty( currentRow, t1, t2, t3, t4, t5, t6 );

            return found ? null : currentRow;
        }

        public T Split<T1, T2, T3, T4, T5, T6, T7>( T t, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7 )
        {
            T currentRow = null;
            bool found = this.mapper.AddRowIfNotExist( t, out currentRow );
            this.mapper.MapProperty( currentRow, t1, t2, t3, t4, t5, t6, t7 );

            return found ? null : currentRow;
        }

        #endregion

        #endregion

        #region Fields Chosing
        private void InitSimpleField()
        {
            var properties = typeof( T ).GetProperties();
            foreach ( var pr in properties )
            {
                if ( pr.PropertyType.IsSimpleType() )
                {
                    simpleFields.Add( pr.Name, new Field() { Name = pr.Name } );
                }
            }
        }

        /// <summary>
        /// chon field, phan tach nhau boi dau phay
        /// </summary>
        /// <param name="fields"></param>
        public void ChoseField( string fields )
        {
            string[] fieldArray = fields.Split( new char[] { ',' } );
            foreach ( var f in fieldArray )
            {
                AddSingleField( f );
            }
        }

        private void AddSingleField( string field )
        {
            Field simpleField = null;
            if ( TryGetSimpleField( field, out simpleField ) )
            {
                this.chosenSimpleFields.Add( field, simpleField );
                this.includedFields.Add( field, true );
                return;
            }
            if ( this.Include( field ) )
            {
                this.includedFields.Add( field, true );
                return;
            }
            throw new Exception( "Field is not existed in entity" );
        }

        public bool ExistField( string field )
        {
            return this.includedFields.ContainsKey( field );
        }

        private bool TryGetSimpleField( string field, out Field configedField )
        {
            if ( simpleFields.TryGetValue( field, out configedField ) )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// include field co quan he navigate
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool Include( string field )
        {
            NavigationField f = null;
            if ( navigationFields.TryGetValue( field, out f ) )
            {
                if ( !chosenNavigationFields.ContainsKey( field ) )
                    chosenNavigationFields.Add( field, f );
                return true;
            }
            return false;
        }
        #endregion

        #region Build query logic
        public virtual string BuildQuery()
        {
            string select = BuildSelectCreteria();
            string from = BuildFromCreteria();
            string where = BuildWhereCreteria();
            string rawSql = string.Format( "{0} from {1} {2}", select, from, where );

            return rawSql;
        }

        public virtual string BuildQuery( string orderBy = null )
        {
            orderBy = string.IsNullOrEmpty( orderBy ) ? keyName : orderBy;
            orderBy = string.Format( "Order By {0}.{1}", this.table, orderBy );
            string select = BuildSelectCreteria();
            string from = BuildFromCreteria();
            string where = BuildWhereCreteria();
            string rawSql = string.Format( "{0} from {1} {2} {3}", select, from, where, orderBy );

            return rawSql;
        }

        public virtual string BuildQuery( string orderBy, int top )
        {
            orderBy = string.IsNullOrEmpty( orderBy ) ? keyName : orderBy;
            orderBy = string.Format( "Order By {0}.{1}", this.table, orderBy );
            string select = BuildSelectCreteria( top );
            string from = BuildFromCreteria();
            string where = BuildWhereCreteria();
            string rawSql = string.Format( "{0} from {1} {2} {3}", select, from, where, orderBy );

            return rawSql;
        }

        public virtual string BuildQuery( string orderBy, int pageIndex, int pageSize )
        {
            orderBy = string.IsNullOrEmpty( orderBy ) ? keyName : orderBy;
            orderBy = string.Format( "Order By {0}.{1}", this.table, orderBy );
            string select = BuildSelectCreteria();
            string from = BuildFromCreteria();
            string where = BuildWhereCreteria();
            string offset = string.Format( "OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", ( pageIndex - 1 ) * pageSize, pageSize );
            string rawSql = string.Format( "{0} from {1} {2} {3} {4}", select, from, where, orderBy, offset );

            return rawSql;
        }

        public virtual string BuildScalar( string operation, string field )
        {
            field = string.Format( "{0}.[{1}]", this.table, field );
            string from = BuildFromCreteria();
            string where = BuildWhereCreteria();
            string rawSql = string.Format( "select {0}({1}) from {2} {3}", operation, field, from, where );

            return rawSql;
        }
        #endregion

        #region Predicate processing
        protected virtual void AddPredicate( Predicate predicate, string parameterName )
        {
            var dictionary = parameters as IDictionary<string, object>;
            if ( !dictionary.ContainsKey( parameterName ) )
            {
                dictionary.Add( parameterName, predicate.Value );
            }
            predicates.Add( predicate );
        }

        protected virtual string Expression( string expression )
        {
            return string.Format( "{0}.{1}", this.table, expression );
        }

        protected virtual string BuildWhereCreteria()
        {
            if ( predicates.Count > 0 )
            {
                string expression = string.Empty;
                foreach ( var p in predicates )
                {
                    if ( string.IsNullOrEmpty( expression ) )
                    {
                        expression += string.Format( "where {0}", p.Expression );
                        continue;
                    }
                    expression += string.Format( " {0} {1}", p.Operator, p.Expression );
                }

                return expression;
            }

            return string.Empty;
        }
        #endregion

        #region Build Select Logics
        protected virtual string BuildSelectCreteria( int top = 0 )
        {
            string s = string.Empty;
            s = BuildSelectEntity( top );
            this.splitOn = BuildSplitOn();
            if ( chosenNavigationFields.Count != 0 )
            {
                s = BuildSelectNavigationField( s );
                return s;
            }

            s = string.Format( "{0},{1}", s, this.BuildSelectSplitOn() );
            return s;
        }

        private string BuildSelectEntity( int top = 0 )
        {
            string s = "select ";
            if ( top != 0 )
            {
                s = string.Format( "{0} TOP {1} ", s, top );
            }
            if ( chosenSimpleFields.Count != 0 )
            {
                s += BuildSelectSimpleField();
                return s;
            }

            s += SelectAll();
            return s;
        }

        protected virtual string BuildSplitOn()
        {
            return string.Join( ",", splitFields.Select( s => s.Keyword ) );
        }

        protected virtual string BuildSelectSplitOn()
        {
            return string.Join( ",", splitFields.Select( f => string.Format( "1 as {0}", f.Keyword ) ) );
        }

        protected string selectNavigationFormat = "1 as {0},{1}";
        protected virtual string BuildSelectNavigationField( string s )
        {
            List<string> listFields = new List<string>();
            foreach ( var field in splitFields )
            {
                var nav = chosenNavigationFields.Where( n => n.Value.SplitOn == field.Keyword ).Select( n => n.Value ).FirstOrDefault();
                if ( nav != null )
                {
                    field.Select = nav.Select;
                }
                if ( !string.IsNullOrEmpty( field.Select ) )
                {
                    listFields.Add( string.Format( selectNavigationFormat, field.Keyword, field.Select ) );
                    continue;
                }

                listFields.Add( string.Format( "1 as {0}", field.Keyword ) );
            }
            return s + "," + string.Join( ",", listFields );
        }

        protected string selectFormat = "{0}.[{1}]";

        protected virtual string BuildSelectSimpleField()
        {
            return string.Join( ",", chosenSimpleFields.Select( k => string.Format( selectFormat, this.table, k.Value.Name ) ) );
        }

        protected virtual string SelectAll()
        {
            return string.Format( "{0}.*", this.table );
        }
        #endregion

        #region Build From Logics
        protected virtual string BuildFromCreteria()
        {
            string fr = this.table;
            if ( chosenNavigationFields.Count != 0 )
            {
                foreach ( var nf in chosenNavigationFields )
                {
                    fr += " " + nf.Value.Relation;
                }
            }
            return fr;
        }
        #endregion

        #region Properties
        public object Paramters
        {
            get { return this.parameters; }
        }

        public string SplitOn
        {
            get { return splitOn; }
            set { splitOn = value; }
        }
        public EntityMapper<T> Mapper
        {
            get { return mapper; }
            set { mapper = value; }
        }

        public Dictionary<string, bool> IncludedFields
        {
            get { return includedFields; }
            set { includedFields = value; }
        }
        #endregion
    }
}
