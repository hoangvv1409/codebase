namespace Infrastructure.Shard.SqlMasterSlave
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Infrastructure.Shard;

    public class ShardClusterDBContext : DbContext
    {
        public ShardClusterDBContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Token> Tokens { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Node>().ToTable("Node");
            modelBuilder.Entity<Node>().HasKey(x => x.NodeID);

            modelBuilder.Entity<Token>().ToTable("Token");

            modelBuilder.Entity<Node>().HasMany(c => c.Tokens).WithRequired().HasForeignKey(x => x.NodeId);
        }
    }
}