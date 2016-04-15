using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapperMapping
{
    [Table( Name = "[dbo].[A]" )]
    public class A
    {
        public int Id { get; set; }
        public string Val { get; set; }
        public int DId { get; set; }
        public D D { get; set; }
        public List<B> ListB { get; set; }
        public List<C> ListC { get; set; }
    }
}
