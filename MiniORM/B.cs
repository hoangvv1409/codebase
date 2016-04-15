using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapperMapping
{
    [Table( Name = "[dbo].[B]" )]
    public class B
    {
        public int Id { get; set; }
        public int AId { get; set; }
        public string Val { get; set; }
    }
}
