using ConceptDbLib.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConceptDbLib
{
    public class ConceptContext : DbContext
    {
        public DbSet<CptObjLibrary> ConceptObjectLibs => Set<CptObjLibrary>();
        public DbSet<CptNetwork> ConceptNetworks => Set<CptNetwork>();

        //REMAINING CODE BELOW
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connString = Environment.GetEnvironmentVariable("connString") ?? throw new InvalidProgramException("Environmental Variable 'connString' not found.");            
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(connString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //LIBRARY -|-----||- OBJECT
            modelBuilder.Entity<CptObject>()
                .HasMany(e => e.Libraries)
                .WithMany(e => e.Objects);

            //NODE -|-----||- OBJECT
            modelBuilder.Entity<CptNetworkNode>()
                .HasOne(e => e.CptObject)
                .WithMany(e => e.Nodes);

            //NODE -||-----||- NODE (children)
            modelBuilder.Entity<CptNetworkNode>()
                .HasMany(e => e.ParentNodes)
                .WithMany(e => e.ChildNodes);

            //NODE -||-----||- NODE (parents)
            modelBuilder.Entity<CptNetworkNode>()
                .HasMany(e => e.ChildNodes)
                .WithMany(e => e.ParentNodes);

            //NODE -|-----||- VALUE
            modelBuilder.Entity<CptNetworkNode>()
                .HasMany(e => e.Values);

            //OBJECT -||----||- VALUE
            modelBuilder.Entity<CptObject>()
                .HasMany(e => e.CptValues)
                .WithMany(e => e.Objects);

            //NETWORK -|-----||- NODE
            modelBuilder.Entity<CptNetwork>()
                .HasMany(e => e.Nodes)
                .WithOne(e => e.Network);
        }
    }
}