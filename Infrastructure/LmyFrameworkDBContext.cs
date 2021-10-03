using Common.Interfaces;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure
{
    public partial class LmyFrameworkDBContext : DbContext
    {
        IRequestContext _requestContext;

        public LmyFrameworkDBContext()
        {

        }

        public LmyFrameworkDBContext(DbContextOptions options, IRequestContext requestContext)
            : base(options)
        {
            _requestContext = requestContext;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                //  optionsBuilder.UseSqlite("Data Source=./DBBackup/khidma.db;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClientErrors>(entity =>
            {
                entity.Property(e => e.ID)
                 .HasMaxLength(128)
                 .ValueGeneratedNever();
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.Property(e => e.ID)
                    .HasMaxLength(128)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.ID)
                    .HasMaxLength(128)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserID);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleID);
            });

            modelBuilder.Entity<Projects>(entity =>
            {
                entity.Property(e => e.ID)
                    .HasMaxLength(128)
                    .ValueGeneratedNever();
            });
        }

        public override int SaveChanges()
        {
            setTracks();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            setTracks();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            setTracks();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void setTracks()
        {
            var entries = ChangeTracker.Entries().Where(x =>
            x.Entity is ITrackable || x.Entity is IUserTrackable
            && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.Entity is ITrackable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        ((ITrackable)entry.Entity).CreationDate = DateTime.UtcNow;
                    }

                    ((ITrackable)entry.Entity).ModifiedDate = DateTime.UtcNow;

                }

                if (entry.Entity is IUserTrackable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        ((IUserTrackable)entry.Entity).CreatedBy = _requestContext.CurrentUserID;
                    }

                    ((IUserTrackable)entry.Entity).ModifiedBy = _requestContext.CurrentUserID;
                }
            }
        }
    }
}
