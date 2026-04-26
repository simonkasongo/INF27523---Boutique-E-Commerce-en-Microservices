using EC_ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace EC_ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<Produit> Produits { get; set; }
    }
}
