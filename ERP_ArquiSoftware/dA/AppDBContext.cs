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

       
    }
}
