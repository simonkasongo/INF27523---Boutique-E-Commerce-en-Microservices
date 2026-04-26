using EC_OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace EC_OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Commande> Commandes { get; set; }
        public DbSet<LigneCommande> LignesCommande { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LigneCommande>()
                .HasOne(l => l.Commande)
                .WithMany(c => c.Lignes)
                .HasForeignKey(l => l.IdCommande)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
