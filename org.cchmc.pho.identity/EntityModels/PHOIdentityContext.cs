﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace org.cchmc.pho.identity.EntityModels
{
    public partial class PHOIdentityContext : DbContext
    {
        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<TlkUserType> TlkUserType { get; set; }

        public PHOIdentityContext(DbContextOptions<PHOIdentityContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>(entity =>
            {
                entity.ToTable("Login", "reg");

                entity.Property(e => e.AccessFailedCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOnDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedBy).HasMaxLength(50);

                entity.Property(e => e.DeletedDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.LegalDisclaimerSigned).HasColumnType("datetime");

                entity.Property(e => e.LockoutFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RefreshToken).HasMaxLength(100);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("Staff", "reg");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ActiveFlag).HasDefaultValueSql("((1))");

                entity.Property(e => e.CchmcProvId)
                    .HasColumnName("CCHMC_PROV_ID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOnDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeletedDate).HasColumnType("datetime");

                entity.Property(e => e.DeletedFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Npi).HasColumnName("NPI");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<TlkUserType>(entity =>
            {
                entity.ToTable("tlkUserType", "reg");

                entity.Property(e => e.Name).HasMaxLength(25);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}