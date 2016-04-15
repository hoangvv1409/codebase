using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Threading;
using MiniOrm;
using System.Linq.Expressions;
using MiniOrm.ORM.QueryBuilders;
using System.IO;
using DapperOrm;

namespace SlapperMapping
{
    class Program
    {
        static void Main( string[] args )
        {
            QueryBuilder();
            TestStationQueryBuilder();
            
            Console.WriteLine( "Finish!" );
            Console.ReadKey();
        }

        private static void TestStationQueryBuilder()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ "TMS" ].ConnectionString;
            using ( SqlConnection conn = new SqlConnection( connectionString ) )
            {
                StationSqlQueryBuilder queryBuilder = new StationSqlQueryBuilder();
                queryBuilder.ChoseField( "Id,Name,StationType,StationWards" );

                string sql = queryBuilder.BuildQuery( "Id desc", 50 );
                //string sql = queryBuilder.BuildScalar( "COUNT", "Id" );
                Console.WriteLine( sql );

                var resutls = conn.Query( sql, queryBuilder.MakeResult, queryBuilder.Paramters, splitOn: queryBuilder.SplitOn ).Where( r => r != null );
                int count = 0;
                foreach ( var r in resutls )
                {
                    count++;
                    Console.WriteLine( "{0}-{1}-{2}", count, r.Id, r.Name );
                }
            }
        }

        private static void QueryBuilder()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ "TMS" ].ConnectionString;
            using ( SqlConnection conn = new SqlConnection( connectionString ) )
            {
                AQueryBuilder queryBuilder = new AQueryBuilder();
                queryBuilder.Id = 1;
                queryBuilder.Val = "A1";
                queryBuilder.ChoseField( "ListC,D,ListB" );

                string sql = queryBuilder.BuildQuery( "Id desc", 1, 50 );
                //string sql = queryBuilder.BuildScalar( "COUNT", "Id" );
                Console.WriteLine( sql );

                var resutls = conn.Query( sql, queryBuilder.MakeResult, queryBuilder.Paramters, splitOn: queryBuilder.SplitOn ).Where( r => r != null );
                int count = 0;
                foreach ( var r in resutls )
                {
                    count++;
                    Console.WriteLine( "{0}-{1}-{2}", count, r.Id, r.Val );
                    //Console.WriteLine( "Number B:{0}", r.ListB.Count );
                    //Console.WriteLine( "Number C:{0}", r.ListC.Count );
                    //Console.WriteLine( "D Value:{0}", r.D.Val );
                    Console.WriteLine( "----------------------------" );
                }
            }
        }
    }
}
