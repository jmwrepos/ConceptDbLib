using CptClientShared.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConceptDbLib
{
    public class ConceptContext : DbContext
    {
        public DbSet<CptLibrary> Libraries => Set<CptLibrary>();
        public DbSet<CptObject> Objects => Set<CptObject>();
        public DbSet<CptProperty> Properties => Set<CptProperty>();
        public DbSet<CptObjectType> ObjectTypes => Set<CptObjectType>();
        public DbSet<CptObjectProperty> ObjectProperties => Set<CptObjectProperty>();
        public DbSet<CptStringValue> StringValues => Set<CptStringValue>();
        public DbSet<CptNumberValue> NumberValues => Set<CptNumberValue>();
        public DbSet<CptObjectValue> ObjectValues => Set<CptObjectValue>();
        //REMAINING CODE BELOW
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connString = Environment.GetEnvironmentVariable("connString") ?? throw new InvalidProgramException("Environmental Variable 'connString' not found.");            
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(connString).EnableSensitiveDataLogging();
        }
        public void DetachAllEntities()
        {
            var changedEntriesCopy = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //E -|-----|- E
            //E -|-----||- E
            //E -||-----||- E
            /*    //LIBRARY -|-----||- OBJECT
                modelBuilder.Entity<Type>()
                    .HasMany(e => e.Collection)
                    .WithMany(e => e.Collection);*/


            modelBuilder.Entity<CptLibrary>()
                .HasMany(e => e.Objects)
                .WithOne(e => e.Library)
                .HasForeignKey(e => e.LibraryId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<CptObject>()
                .HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}