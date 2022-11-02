using CptClientShared.Entities;
using CptClientShared.Entities.Accounting;
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
        public DbSet<CptObjectNameValue> ObjNameValues => Set<CptObjectNameValue>();
        public DbSet<CptStringValue> StringValues => Set<CptStringValue>();
        public DbSet<CptNumberValue> NumberValues => Set<CptNumberValue>();
        public DbSet<CptAccount> Accounts => Set<CptAccount>();
        public DbSet<CptAcctType> AccountTypes => Set<CptAcctType>();
        public DbSet<CptAcctUser> AccountUsers => Set<CptAcctUser>();
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

            modelBuilder.Entity<CptLibrary>()
                .HasMany(e => e.ObjectTypes)
                .WithOne(e => e.ParentLibrary)
                .HasForeignKey(e => e.ParentLibraryId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<CptObjectType>()
                .HasOne(e => e.ParentType)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<CptObjectType>()
                .HasMany(e => e.Objects)
                .WithMany(e => e.ObjectTypes);

            modelBuilder.Entity<CptLibrary>()
                .HasMany(e => e.Properties)
                .WithOne(e => e.Library)
                .HasForeignKey(e => e.LibraryId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<CptObject>()
                .HasMany(e => e.ObjectProperties)
                .WithOne(e => e.Object)
                .HasForeignKey(e => e.ObjectId);

            modelBuilder.Entity<CptProperty>()
                .HasMany(e => e.ObjectProperties)
                .WithOne(e => e.Property)
                .HasForeignKey(e => e.PropertyId);

            modelBuilder.Entity<CptObjectProperty>()
                .HasMany(e => e.NumberValues)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<CptObjectProperty>()
                .HasMany(e => e.StringValues)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<CptObjectProperty>()
                .HasMany(e => e.ObjNameValues)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId);

            //ACCOUNTING

            modelBuilder.Entity<CptAcctType>()
                .HasMany(e => e.Accounts)
                .WithOne(e => e.AccountType)
                .HasForeignKey(e => e.AccountTypeId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<CptAccount>()
                .HasMany(e => e.Users)
                .WithOne(e => e.Account)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<CptAccount>()
                .HasMany(e => e.Libraries)
                .WithOne(e => e.Account)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}