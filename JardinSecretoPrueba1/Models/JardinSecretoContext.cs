using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JardinSecretoPrueba1.Models;

public partial class JardinSecretoContext : DbContext
{
    public JardinSecretoContext()
    {
    }

    public JardinSecretoContext(DbContextOptions<JardinSecretoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Extra> Extras { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Sabor> Sabors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=JAIME\\MSSQLSERVER02;Database=JardinSecreto;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Administ__43AA41415999BA30");

            entity.ToTable("Administrador");

            entity.HasIndex(e => e.Usuario, "UQ__Administ__9AFF8FC6E0DBC45D").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.ContraseñaHash)
                .HasMaxLength(255)
                .HasColumnName("contraseña_hash");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .HasColumnName("usuario");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__DB875A4FD2104F6B");

            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Extra>(entity =>
        {
            entity.HasKey(e => e.ExtraId).HasName("PK__Extra__D482B9466C540644");

            entity.ToTable("Extra");

            entity.Property(e => e.ExtraId).HasColumnName("Extra_id");
            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.Nombre).HasMaxLength(75);
            entity.Property(e => e.PrecioExtra)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio_Extra");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Extras)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Extra__Id_Produc__7E37BEF6");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__FB5CEEEC6A225AE7");

            entity.ToTable("Producto");

            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Disponible)
                .HasDefaultValue(true)
                .HasColumnName("disponible");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .HasConstraintName("FK_Producto_Categoria");
        });

        modelBuilder.Entity<Sabor>(entity =>
        {
            entity.HasKey(e => e.SaborId).HasName("PK__Sabor__6C8EB133A41295C2");

            entity.ToTable("Sabor");

            entity.Property(e => e.SaborId).HasColumnName("Sabor_id");
            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.Nombre).HasMaxLength(75);
            entity.Property(e => e.PrecioSabor)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio_Sabor");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Sabores)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sabor__Id_Produc__7B5B524B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
