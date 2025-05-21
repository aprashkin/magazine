using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace magazine_music.Context;

public partial class User9Context : DbContext
{
    public User9Context()
    {
    }

    public User9Context(DbContextOptions<User9Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Instrument> Instruments { get; set; }

    public virtual DbSet<Instrumentimage> Instrumentimages { get; set; }

    public virtual DbSet<InstrumentsType> InstrumentsTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=45.67.56.214;Port=5666;Database=user9;Username=user9;Password=X8C8NTnD");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("brands_pkey");

            entity.ToTable("brands", "magazine");

            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandCountry)
                .HasMaxLength(100)
                .HasColumnName("brand_country");
            entity.Property(e => e.BrandDescription).HasColumnName("brand_description");
            entity.Property(e => e.BrandName)
                .HasMaxLength(255)
                .HasColumnName("brand_name");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("genders_pkey");

            entity.ToTable("genders", "magazine");

            entity.Property(e => e.GenderId).HasColumnName("gender_id");
            entity.Property(e => e.GenderName)
                .HasMaxLength(50)
                .HasColumnName("gender_name");
        });

        modelBuilder.Entity<Instrument>(entity =>
        {
            entity.HasKey(e => e.InstrumentId).HasName("instruments_pkey");

            entity.ToTable("instruments", "magazine");

            entity.Property(e => e.InstrumentId).HasColumnName("instrument_id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.InstrumentDescription).HasColumnName("instrument_description");
            entity.Property(e => e.InstrumentName)
                .HasMaxLength(255)
                .HasColumnName("instrument_name");
            entity.Property(e => e.InstrumentPrice)
                .HasPrecision(10, 2)
                .HasColumnName("instrument_price");
            entity.Property(e => e.InstrumentQuantity).HasColumnName("instrument_quantity");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Brand).WithMany(p => p.Instruments)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("instruments_brand_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Instruments)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("instruments_type_id_fkey");
        });

        modelBuilder.Entity<Instrumentimage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("instrumentimages", "magazine");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(255)
                .HasColumnName("image_path");
            entity.Property(e => e.InstrumentId).HasColumnName("instrument_id");

            entity.HasOne(d => d.Instrument).WithMany()
                .HasForeignKey(d => d.InstrumentId)
                .HasConstraintName("instrumentimages_instrument_id_fkey");
        });

        modelBuilder.Entity<InstrumentsType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("instruments_type_pkey");

            entity.ToTable("instruments_type", "magazine");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders", "magazine");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.InstrumentId).HasColumnName("instrument_id");
            entity.Property(e => e.OrderDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Instrument).WithMany(p => p.Orders)
                .HasForeignKey(d => d.InstrumentId)
                .HasConstraintName("orders_instrument_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("orders_status_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_user_id_fkey");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("order_items_pkey");

            entity.ToTable("order_items", "magazine");

            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.InstrumentId).HasColumnName("instrument_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Instrument).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.InstrumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_instrument_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_order_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles", "magazine");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("statuses_pkey");

            entity.ToTable("statuses", "magazine");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users", "magazine");

            entity.HasIndex(e => e.UserEmail, "users_user_email_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.GenderId).HasColumnName("gender_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserBirthday).HasColumnName("user_birthday");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .HasColumnName("user_email");
            entity.Property(e => e.UserFirstname)
                .HasMaxLength(100)
                .HasColumnName("user_firstname");
            entity.Property(e => e.UserLastname)
                .HasMaxLength(100)
                .HasColumnName("user_lastname");
            entity.Property(e => e.UserPassword).HasColumnName("user_password");

            entity.HasOne(d => d.Gender).WithMany(p => p.Users)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("users_gender_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
