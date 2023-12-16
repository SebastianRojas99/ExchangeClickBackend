using Microsoft.EntityFrameworkCore;
using ExchangeClick.Entities;

namespace ExchangeClick
{
    public class ExchangeClickContext : DbContext
    {
        public ExchangeClickContext(DbContextOptions<ExchangeClickContext> options) : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        
        // Otras entidades relacionadas

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configura SQLite como el proveedor de base de datos
            optionsBuilder.UseSqlite("Data Source=exchangeclick.db");
        }
    }
}
