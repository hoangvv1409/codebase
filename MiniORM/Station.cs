using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapperMapping
{
    [Table( Name = "[master].[stations]" )]
    public class Station
    {
        public Station()
        {
            //StationWards = new List<StationWard>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string SCode { get; set; }
        public bool IsCashie { get; set; }
        public int UIDChiefStation { get; set; }
        public int CityID { get; set; }
        public Nullable<int> DistID { get; set; }
        public Nullable<int> WardID { get; set; }
        public Nullable<long> StreetID { get; set; }
        public string HouseN0 { get; set; }
        public double Latitude { get; set; }
        public string Note { get; set; }
        public double Longitude { get; set; }
        public bool Visible { get; set; }
        public Nullable<int> UIDCreate { get; set; }
        public System.DateTime CreateDatetime { get; set; }
        public string FullAddress { get; set; }
        public string Phone { get; set; }
        public bool SOS { get; set; }
        public Nullable<int> LongestDistance { get; set; }
        public Nullable<int> TimeExecute { get; set; }
        public int StationTypeId { get; set; }
        public StationType StationType { get; set; }
        public List<StationWard> StationWards { get; set; }
    }
}
