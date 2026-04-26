using EC_PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EC_PaymentService.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

        public DbSet<Paiement> Paiements { get; set; }
    }
}
