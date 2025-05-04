using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RestaurantWebsite.Models;

public partial class RestaurantContext : DbContext
{
    public RestaurantContext()
    {
    }

    public RestaurantContext(DbContextOptions<RestaurantContext> options)
        : base(options)
    {
    }
    public List<Dish> NewDishes { get; set; }
    public List<Dish> OtherDishes { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DiningTable> DiningTables { get; set; }

    public virtual DbSet<Dish> Dishes { get; set; }

    public virtual DbSet<DishCategory> DishCategories { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<OrderDish> OrderDishes { get; set; }

    public virtual DbSet<OrderService> OrderServices { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }
    public virtual DbSet<TableReservation> TableReservations { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-GIDJRRBE;Initial Catalog=Restaurant;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B80945F36C");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CustomerName).HasMaxLength(255);
            entity.Property(e => e.Img)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DiningTable>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__DiningTa__7D5F018ECCEC10BC");

            entity.ToTable("DiningTable");

            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.DishId).HasName("PK__Dish__18834F70E1F1EBBC");

            entity.ToTable("Dish");

            entity.Property(e => e.DishId).HasColumnName("DishID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.DishName).HasMaxLength(255);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Dishes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Dish__CategoryID__4E88ABD4");
        });

        modelBuilder.Entity<DishCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__DishCate__19093A2B317CB77F");

            entity.ToTable("DishCategory");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(255);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1F9D405A9");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Img)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF629C47564");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Feedback__Custom__628FA481");
        });

        modelBuilder.Entity<OrderDish>(entity =>
        {
            entity.HasKey(e => new { e.ServiceId, e.DishId }).HasName("PK__OrderDis__D493841D61EBD18D");

            entity.ToTable("OrderDish");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.DishId).HasColumnName("DishID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Dish).WithMany(p => p.OrderDishes)
                .HasForeignKey(d => d.DishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDish__DishI__59063A47");

            entity.HasOne(d => d.Service).WithMany(p => p.OrderDishes)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDish__Servi__5812160E");
        });

        modelBuilder.Entity<OrderService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__OrderSer__C51BB0EAFF9388DD");

            entity.ToTable("OrderService");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.TableId).HasColumnName("TableID");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderServices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderServ__Custo__5441852A");

            entity.HasOne(d => d.Table).WithMany(p => p.OrderServices)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderServ__Table__534D60F1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A589CAF58D8");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.PaymentTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Employee).WithMany(p => p.Payments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Employe__5EBF139D");

            entity.HasOne(d => d.Service).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Service__5DCAEF64");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserAcco__1788CCAC07B8648B");

            entity.ToTable("UserAccount");

            entity.HasIndex(e => e.Username, "UQ__UserAcco__536C85E4D9538BD6").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Customer");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__UserAccou__Custo__49C3F6B7");

            entity.HasOne(d => d.Employee).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__UserAccou__Emplo__48CFD27E");
        });


        // Add this to the OnModelCreating method in RestaurantContext
        modelBuilder.Entity<TableReservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__TableRes__B7EE5F0425FB61A3");

            entity.ToTable("TableReservation");

            entity.Property(e => e.ReservationId).HasColumnName("ReservationID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.TableReservations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TableRes__Custo__67DE6983");

            entity.HasOne(d => d.Table).WithMany(p => p.TableReservations)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TableRes__Table__66EA454A");

            entity.HasOne(d => d.Employee).WithMany(p => p.TableReservations)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__TableRes__Employee__68D29DBC");
            entity.HasOne(d => d.OrderService)
      .WithMany(p => p.TableReservations)
      .HasForeignKey(d => d.ServiceId)
      .OnDelete(DeleteBehavior.SetNull);


        });
        OnModelCreatingPartial(modelBuilder);
    }

    internal IEnumerable<DiningTable> ToList()
    {
        throw new NotImplementedException();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
