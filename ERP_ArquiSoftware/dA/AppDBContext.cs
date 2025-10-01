using Microsoft.EntityFrameworkCore;
using ERP_ArquiSoftware.Models;


namespace ERP_ArquiSoftware.dA
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict); // evita borrar categoría con productos

            // Relación Proveedor -> Categoria
            modelBuilder.Entity<Proveedor>()
                .HasOne(p => p.Categoria)
                .WithMany()  // si quieres, puedes agregar una lista de Proveedores en Categoria
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.NIT)
                .IsUnique();
        }


    }
}
