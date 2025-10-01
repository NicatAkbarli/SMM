using Microsoft.EntityFrameworkCore;
using SMMPanel.Models;

namespace SMMPanel.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        


    }
}
