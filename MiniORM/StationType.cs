using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapperMapping
{
    [Table( Name = "[master].[StationType]" )]
    public class StationType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Visible { get; set; }
        public int UIDCreate { get; set; }
        public System.DateTime CreateDatetime { get; set; }
    }
}
