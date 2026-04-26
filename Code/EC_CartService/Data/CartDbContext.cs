using EC_CartService.Models;
using Microsoft.EntityFrameworkCore;

namespace EC_CartService.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

        public DbSet<ArticlePanier> ArticlesPanier { get; set; }
    }
}
