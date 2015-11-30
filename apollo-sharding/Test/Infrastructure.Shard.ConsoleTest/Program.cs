using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Shard.HashingFunction;
using Infrastructure.Shard.Repository;
using Infrastructure.Shard.SqlMasterSlave;

namespace Infrastructure.Shard.ConsoleTest
{
    class Program
    {
        private ShardService shardService;
        static void Main(string[] args)
        {
            Database.SetInitializer<ShardClusterDBContext>(null);
            Program p = new Program();
            INodeRepository nodeRepository = new NodeRepository("user id=adruserfortest;password=adruserfortest@qaz;server=10.220.75.25;database=ShardOrderMaster");
            p.shardService = new ShardService(nodeRepository, new MurmurHash2(), 20);

            //p.shardService.AddNode("user id=adruserfortest;password=adruserfortest@qaz;server=10.220.75.25;database=ShardOrder1");
            //p.shardService.AddNode("user id=adruserfortest;password=adruserfortest@qaz;server=10.220.75.25;database=ShardOrder2");
            //p.shardService.AddNode("user id=adruserfortest;password=adruserfortest@qaz;server=10.220.75.25;database=ShardOrder3");
            //p.shardService.AddNode("user id=adruserfortest;password=adruserfortest@qaz;server=10.220.75.25;database=ShardOrder4");

            var tasks = new List<Task>();
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                for (int i = 0; i < 200; i++)
                {
                    tasks.Add(Task.Factory.StartNew(p.Request));
                }

                Task.Factory.ContinueWhenAll(tasks.ToArray(), t =>
                {
                    watch.Stop();
                    Console.WriteLine("Timer: {0}:{1}.{2}", watch.Elapsed.Minutes, watch.Elapsed.Seconds,
                        watch.Elapsed.Milliseconds);
                    //Console.WriteLine("Total Timeout: " + p.exception.Count);
                    //foreach (var v in p.hashDic)
                    //{
                    //    Console.WriteLine("{0}-{1} \t Ratio: {2}%", v.Key, v.Value.Count,
                    //        ((decimal)v.Value.Count / (decimal)400000) * (decimal)100);
                    //}
                });
            }
            catch
            {

            }
            while (true) { }
        }

        private void Request()
        {
            try
            {
                Parallel.For(0, 5000, t =>
                {
                    string key = Guid.NewGuid().ToString();

                    shardService.ClusterManager.Post(key,
                        (connectionString) =>
                        {
                            //Console.WriteLine(connectionString);
                            using (var con = new SqlConnection(connectionString))
                            {
                                con.Open();
                                string query = "INSERT INTO [Order] (OrderGuid, CreateDate) VALUES ('" + key + "', '" + DateTime.Now + "')";
                                var cmd = new SqlCommand(query, con);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    );

                    Order order = shardService.ClusterManager.Get(key,
                        (connectionString) =>
                        {
                            try
                            {
                                using (var con = new SqlConnection(connectionString))
                                {
                                    string guid = string.Empty;
                                    //DateTime dateTime = DateTime.Now;
                                    con.Open();
                                    string query = "SELECT * FROM [Order] WHERE OrderGuid= '" + key + "'";
                                    var cmd = new SqlCommand(query, con);
                                    var reader = cmd.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        guid = reader["OrderGuid"].ToString();
                                        //dateTime = Convert.ToDateTime(reader["DateTime"].ToString());
                                    }

                                    return new Order()
                                    {
                                        Guid = new Guid(guid),
                                        //DateTime = dateTime
                                    };
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                return null;
                            }
                        }
                    );

                    Console.WriteLine("VIEW " + order.Guid);

                    shardService.ClusterManager.Delete(key,
                        (connectionString) =>
                        {
                            using (var con = new SqlConnection(connectionString))
                            {
                                con.Open();
                                string query = "DELETE FROM [Order] WHERE OrderGuid= '" + key + "'";
                                var cmd = new SqlCommand(query, con);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    );
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class Order
    {
        public Guid Guid { get; set; }

        public DateTime DateTime { get; set; }
    }
}
