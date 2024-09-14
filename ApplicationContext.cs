using Microsoft.EntityFrameworkCore;
using CoffeeShop.Entities;

namespace CoffeeShop;

public class ApplicationContext : DbContext
{
    public DbSet<Coffee> Coffees { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    public ApplicationContext() => Database.EnsureCreated();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coffee>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Coffee>().HasData(

            new Coffee { Name = "Espresso", Price = 2.50 },
            new Coffee { Name = "Cappuccino", Price = 3.50 },
            new Coffee { Name = "Latte", Price = 4.00 },
            new Coffee { Name = "Americano", Price = 2.80 },
            new Coffee { Name = "Matcha", Price = 4.50 }

        );

        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CoffeeShopDB;Trusted_Connection=True;");
    }

    public async Task AddCoffeeAsync(string name, double price)
    {
        await Coffees.AddAsync(new Coffee { Name = name, Price = price });
        await SaveChangesAsync();
    }
    public async Task AddOrderAsync(string coffeeName)
    {
        var tempCoffee = await Coffees.FirstOrDefaultAsync(c => c.Name == coffeeName);
        
        if (tempCoffee == null)
            throw new ArgumentException(coffeeName + "not found");
        
        await Orders.AddAsync(new Order{ Coffee = tempCoffee});
        await SaveChangesAsync();
    }

    public IEnumerable<Order> GetAllOrders() => Orders.Include(o => o.Coffee);

    public IEnumerable<Order> GetOrdersAfter(DateTime dateTime) => Orders.Where(o => o.Time > dateTime).Include(o => o.Coffee);

    public async Task DeleteOrder(string orderId)
    {
        var tempOrder = await Orders.FirstOrDefaultAsync(o => o.Id == Guid.Parse(orderId));
        if (tempOrder == null)
            throw new ArgumentException("Not found order with order id:" + orderId);
        Orders.Remove(tempOrder);
    }

}