using EC_NotificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace EC_NotificationService.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }
    }
}
