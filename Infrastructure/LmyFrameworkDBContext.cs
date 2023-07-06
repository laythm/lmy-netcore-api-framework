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

        public LmyFrameworkDBContext()
        {

        }

        public LmyFrameworkDBContext(DbContextOptions options)
            : base(options)
        {
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

            modelBuilder.Entity<Roles>()
              .HasData(
               new Roles { ID = "Admin", Name = "Admin" },
               new Roles { ID = "ProjectManager", Name = "ProjectManager" });

            modelBuilder.Entity<Users>()
              .HasData(
               new Users
               {
                   ID = "admin",
                   FirstName = "admin",
                   LastName = "admin",
                   UserName = "admin",
                   Email = "admin@admin.com",
                   Password = "AM7eHj7czL/Fx6sC1hg6C19+h5MesLGZ9NRpgw81D3FuifHlPqn5ha/z2NzwW6iu3g==",//admin
                   IsDeleted = false,
                   CreatedBy = "admin",
                   ModifiedBy = "admin",
                   CreationDate = DateTime.Now,
                   ModifiedDate = DateTime.Now
               }); 
        }
    }
}
