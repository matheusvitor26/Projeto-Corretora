using System.Data.Entity;

namespace Corretora.Models
{
    public class CorretoraContext : DbContext
    {
        public CorretoraContext() : base("name=Corretora") { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}