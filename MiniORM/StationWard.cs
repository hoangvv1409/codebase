using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapperMapping
{
    [Table( Name = "[master].[StationWard]" )]
    public class StationWard
    {
        public int StationId { get; set; }
        public int WardId { get; set; }
        public Nullable<int> CityId { get; set; }
    }
}
