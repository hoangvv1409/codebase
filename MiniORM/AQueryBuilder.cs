using DapperOrm;
using SlapperMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniOrm.ORM.QueryBuilders
{
    public class AQueryBuilder : QueryBuilder<A>
    {
        public AQueryBuilder()
            : base()
        {
            this.HasKey( "Id", a => a.Id );
            NavigationField<A, B> nav1 = new NavigationField<A, B>( "ListB", "Id", "AId", b => b.Id );
            NavigationField<A, C> nav2 = new NavigationField<A, C>( "ListC", "Id", "AId", c => c.Id );
            NavigationField<A, D> nav3 = new NavigationField<A, D>( "D", "DId", "Id", d => d.Id );
            this.Navigate( nav1, nav2, nav3 );

            this.MakeResult = Split;
        }

        private int id;
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                Predicate predicate = new Predicate( Expression( "Id = @Id" ), SqlLogicOperator.And, value );
                AddPredicate( predicate, "Id" );
            }
        }

        private string val;
        public string Val
        {
            get { return val; }
            set
            {
                val = value;
                Predicate predicate = new Predicate( Expression( "Val = @Val" ), SqlLogicOperator.Or, value );
                AddPredicate( predicate, "Val" );
            }
        }

        public Func<A, B, C, D, A> MakeResult { get; private set; }

        public A MapperResult( A a, B b, C c, D d )
        {
            var mapper = this.Mapper;
            A currentRow = null;
            bool found = mapper.AddRowIfNotExist( a, out currentRow );

            if ( this.ExistField( "ListB" ) )
            {
                mapper.Map( currentRow, "ListB", b, f => f.Id );
            }
            if ( this.ExistField( "ListC" ) )
            {
                mapper.Map( currentRow, "ListC", c, f => f.Id );
            }
            if ( this.ExistField( "D" ) )
            {
                mapper.Map( currentRow, "D", d, f => f.Id );
            }

            return found ? null : currentRow;
        }
    }
}
