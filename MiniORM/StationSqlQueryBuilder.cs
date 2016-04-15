using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DapperOrm;

namespace SlapperMapping
{
    public class StationSqlQueryBuilder : QueryBuilder<Station>
    {
        public StationSqlQueryBuilder()
        {
            this.HasKey( "Id", t => t.Id );
            NavigationField<Station, StationType> nav1 = new NavigationField<Station, StationType>( "StationType", "StationTypeId", "Id", s => s.Id );
            NavigationField<Station, StationWard> nav2 = new NavigationField<Station, StationWard>( "StationWards", "Id", "StationId", s => new { StationId = s.StationId, WardId = s.WardId } );
            this.Navigate( nav1, nav2 );
        }

        public Func<Station, StationType, StationWard, Station> MakeResult
        {
            get { return this.Split; }
        }
    }
}
