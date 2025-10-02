using Microsoft.EntityFrameworkCore;
using ERP_ArquiSoftware.Models;
using ERP_ArquiSoftware.Models.Inventario;

namespace ERP_ArquiSoftware.dA
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options) { }

        // Existentes
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<ProductoProveedor> ProductoProveedores { get; set; }

        // Nuevas tablas
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<UnidadMedida> UnidadesMedida { get; set; }
        public DbSet<Impuesto> Impuestos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Existencia> Existencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Relaciones básicas ----------
            // Producto -> Categoria (N:1)
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto -> Marca (N:1, opcional)
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Marca)
                .WithMany(m => m.Productos)
                .HasForeignKey(p => p.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto -> UnidadMedida (N:1, opcional)
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.UnidadMedida)
                .WithMany()
                .HasForeignKey(p => p.UnidadMedidaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto -> Impuesto (N:1, opcional)
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Impuesto)
                .WithMany()
                .HasForeignKey(p => p.ImpuestoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Proveedor ----------
            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.NIT)
                .IsUnique();

            // ---------- Producto <-> Proveedor (N:N con histórico) ----------
            modelBuilder.Entity<ProductoProveedor>()
                .HasKey(pp => new { pp.ProductoId, pp.ProveedorId, pp.FechaDesde });

            modelBuilder.Entity<ProductoProveedor>()
                .HasOne(pp => pp.Producto)
                .WithMany(p => p.ProductoProveedores)
                .HasForeignKey(pp => pp.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductoProveedor>()
                .HasOne(pp => pp.Proveedor)
                .WithMany(p => p.ProductoProveedores)
                .HasForeignKey(pp => pp.ProveedorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductoProveedor>()
                .HasIndex(pp => new { pp.ProductoId, pp.EsProveedorPrincipal });

            modelBuilder.Entity<ProductoProveedor>()
                .HasIndex(pp => pp.SkuProveedor);

            // ---------- Producto (índices/constraints) ----------
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.NombreProducto);

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Gtin);

            // ---------- Stock por Almacén ----------
            modelBuilder.Entity<Existencia>()
                .HasKey(e => new { e.ProductoId, e.AlmacenId });

            modelBuilder.Entity<Existencia>()
                .HasOne(e => e.Producto)
                .WithMany(p => p.Existencias)
                .HasForeignKey(e => e.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Existencia>()
                .HasOne(e => e.Almacen)
                .WithMany(a => a.Existencias)
                .HasForeignKey(e => e.AlmacenId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- Precisión decimal ----------
            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioUnitario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Producto>()
                .Property(p => p.CostoEstandar).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Producto>()
                .Property(p => p.CostoPromedio).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Producto>()
                .Property(p => p.UltimoCosto).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductoProveedor>()
                .Property(pp => pp.PrecioCompra).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Impuesto>()
                .Property(i => i.Porcentaje)
                .HasColumnType("decimal(5,2)");
        }
    }
}
