using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DocTrackerEFModels.EFModels;

public partial class DocTrackerDbContext : DbContext
{
    public DocTrackerDbContext()
    {
    }

    public DocTrackerDbContext(DbContextOptions<DocTrackerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<ReadingLog> ReadingLogs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocId);

            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<ReadingLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.Property(e => e.ClientIp)
                .HasMaxLength(50)
                .HasColumnName("ClientIP");
            entity.Property(e => e.CreatedTime).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Doc).WithMany(p => p.ReadingLogs)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReadingLogs_Documents");

            entity.HasOne(d => d.User).WithMany(p => p.ReadingLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReadingLogs_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Account).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(150);
            entity.Property(e => e.PictureUrl)
                .HasMaxLength(500)
                .HasColumnName("PictureURL");
            entity.Property(e => e.UserName).HasMaxLength(20);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
