using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace angPOC.Data
{
    public partial class UserDBContext : DbContext
    {
        public UserDBContext()
        {
        }

        public UserDBContext(DbContextOptions<UserDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppPermission> AppPermissions { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:UserDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AppPermission>(entity =>
            {
                entity.HasKey(e => e.ApId)
                    .HasName("PK_Permissions");

                entity.ToTable("AppPermission");

                entity.Property(e => e.ApId).HasColumnName("apID");

                entity.Property(e => e.ApActive)
                    .IsRequired()
                    .HasColumnName("apActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ApCreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("apCreatedBy")
                    .HasDefaultValueSql("('admin')");

                entity.Property(e => e.ApCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("apCreatedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ApModifiedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("apModifiedBy")
                    .HasDefaultValueSql("('admin')");

                entity.Property(e => e.ApModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("apModifiedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AppId)
                    .HasColumnName("appID")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PermName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("permName");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.AppPermissions)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Application");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.AppId)
                    .HasName("PK_Applications");

                entity.ToTable("Application");

                entity.Property(e => e.AppId).HasColumnName("appID");

                entity.Property(e => e.AppActive)
                    .IsRequired()
                    .HasColumnName("appActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.AppCreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("appCreatedBy");

                entity.Property(e => e.AppCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("appCreatedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AppFlags).HasColumnName("appFlags");

                entity.Property(e => e.AppModifiedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("appModifiedBy");

                entity.Property(e => e.AppModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("appModifiedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AppName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("appName");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("State");

                entity.HasIndex(e => e.Code, "NonClusteredIndex-20211205-143404")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UsrId)
                    .HasName("PK_Users");

                entity.ToTable("User");

                entity.HasIndex(e => e.UsrLogin, "UX_UsrLogin")
                    .IsUnique();

                entity.Property(e => e.UsrId).HasColumnName("usrID");

                entity.Property(e => e.UsrActive)
                    .IsRequired()
                    .HasColumnName("usrActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UsrClock)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("usrClock");

                entity.Property(e => e.UsrCreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("usrCreatedBy");

                entity.Property(e => e.UsrCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("usrCreatedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UsrEmail)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("usrEmail");

                entity.Property(e => e.UsrFirstName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("usrFirstName");

                entity.Property(e => e.UsrLastName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("usrLastName");

                entity.Property(e => e.UsrLogin)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("usrLogin");

                entity.Property(e => e.UsrModifiedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("usrModifiedBy");

                entity.Property(e => e.UsrModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("usrModifiedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UsrStateId).HasColumnName("usrStateID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
