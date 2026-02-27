using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using fishapp.Models;
using Microsoft.EntityFrameworkCore;

namespace fishapp.Services;

public partial class FishContext : DbContext
{
    public FishContext(DbContextOptions<FishContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<Postavshik> Postavshiks { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Proisvoditel> Proisvoditels { get; set; }

    public virtual DbSet<Punkt> Punkts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public FishContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = App.Configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07911C44D9");

            entity.ToTable("Category");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07E0D23CB9");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdPunktNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdPunkt)
                .HasConstraintName("FK__Orders__IdPunkt__5EBF139D");

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdStatus)
                .HasConstraintName("FK__Orders__IdStatus__60A75C0F");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__Orders__IdUser__5FB337D6");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderPro__3214EC070C136458");

            entity.ToTable("OrderProduct");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.IdOrder)
                .HasConstraintName("FK__OrderProd__IdOrd__68487DD7");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK__OrderProd__IdPro__693CA210");
        });

        modelBuilder.Entity<Postavshik>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Postavsh__3214EC07CA8390B2");

            entity.ToTable("Postavshik");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07D97ED46C");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .HasConstraintName("FK__Products__IdCate__656C112C");

            entity.HasOne(d => d.IdPostavshikNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdPostavshik)
                .HasConstraintName("FK__Products__IdPost__6477ECF3");

            entity.HasOne(d => d.IdProisvoditelNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdProisvoditel)
                .HasConstraintName("FK__Products__IdProi__6383C8BA");
        });

        modelBuilder.Entity<Proisvoditel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Proisvod__3214EC0769468607");

            entity.ToTable("Proisvoditel");
        });

        modelBuilder.Entity<Punkt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Punkt__3214EC075FD1DD2C");

            entity.ToTable("Punkt");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC072D0338A0");

            entity.ToTable("Role");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Status__3214EC07F455EBEA");

            entity.ToTable("Status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07FD81E4B9");

            entity.Property(e => e.Fio).HasColumnName("FIO");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .HasConstraintName("FK__Users__IdRole__5441852A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
