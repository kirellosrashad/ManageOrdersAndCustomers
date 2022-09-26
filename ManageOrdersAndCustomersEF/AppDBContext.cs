using ManageOrdersAndCustomersBL;
using Microsoft.EntityFrameworkCore;
using System;

namespace ManageOrdersAndCustomersEF
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base (options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"data source=localhost;initial catalog=OrderManager;integrated security=True;");
        }

        
        public DbSet<Order> Order { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Customer> Customer { get; set; }
    }
}
