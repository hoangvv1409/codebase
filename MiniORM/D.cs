using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapperMapping
{
    [Table( Name = "[dbo].[D]" )]
    public class D
    {
        public int Id { get; set; }
        public string Val { get; set; }
    }
}
