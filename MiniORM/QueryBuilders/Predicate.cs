using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperOrm
{
    public class Predicate
    {
        public Predicate()
        { }

        public Predicate( string expression, SqlLogicOperator sqlOperator, object value )
        {
            this.Expression = expression;
            this.Operator = sqlOperator.ToString();
            this.Value = value;
        }

        public string Expression { get; private set; }
        public string Operator { get; private set; }
        public object Value { get; private set; }
    }
}
