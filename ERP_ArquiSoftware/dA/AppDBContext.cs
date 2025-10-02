using Microsoft.EntityFrameworkCore;
using ERP_ArquiSoftware.Models;                 // Proveedor, Categoria (si siguen en este ns)
using ERP_ArquiSoftware.Models.Inventario;      // Producto, Marca, UnidadMedida, Impuesto, Almacen, Existencia
using ERP_ArquiSoftware.Models.RRHH;            // Empleado, Departamento, Cargo, Contrato, TipoContrato

namespace ERP_ArquiSoftware.dA
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options) { }

        // --------- Inventario / Proveedores ---------
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<ProductoProveedor> ProductoProveedores { get; set; }

        public DbSet<Marca> Marcas { get; set; }
        public DbSet<UnidadMedida> UnidadesMedida { get; set; }
        public DbSet<Impuesto> Impuestos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Existencia> Existencias { get; set; }

        // --------- RRHH ---------
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<TipoContrato> TiposContrato { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===================== INVENTARIO =====================

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

            // Proveedor: NIT único
            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.NIT)
                .IsUnique();

            // Producto <-> Proveedor con histórico (PK compuesta incluye FechaDesde)
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

            // Índices de Producto
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.NombreProducto);
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Sku).IsUnique();
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Gtin);

            // Stock por Almacén
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

            // Precisión decimal Inventario
            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioUnitario).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Producto>()
                .Property(p => p.CostoEstandar).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Producto>()
                .Property(p => p.CostoPromedio).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Producto>()
                .Property(p => p.UltimoCosto).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ProductoProveedor>()
                .Property(pp => pp.PrecioCompra).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Impuesto>()
                .Property(i => i.Porcentaje).HasColumnType("decimal(5,2)");

            // ======================= RRHH ========================

            // Empleado -> Almacén (N:1)
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Almacen)
                .WithMany(a => a.Empleados)           // si tu modelo Almacen no tiene Empleados, usa .WithMany()
                .HasForeignKey(e => e.AlmacenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Empleado -> Departamento (N:1)
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Departamento)
                .WithMany(d => d.Empleados)
                .HasForeignKey(e => e.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Empleado -> Cargo (N:1)
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Cargo)
                .WithMany(c => c.Empleados)
                .HasForeignKey(e => e.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Documento único (opcional pero recomendable)
            modelBuilder.Entity<Empleado>()
                .HasIndex(e => e.Documento)
                .IsUnique();

            // Contrato -> Empleado (N:1)
            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Empleado)
                .WithMany(e => e.Contratos)
                .HasForeignKey(c => c.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contrato -> TipoContrato (N:1)
            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.TipoContrato)
                .WithMany(t => t.Contratos)
                .HasForeignKey(c => c.TipoContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Precisión monetaria RRHH
            modelBuilder.Entity<Cargo>()
                .Property(c => c.SalarioBase)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Contrato>()
                .Property(c => c.Salario)
                .HasColumnType("decimal(18,2)");
        }
    }
}
