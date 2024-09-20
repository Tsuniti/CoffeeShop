using Microsoft.EntityFrameworkCore;
using CoffeeShop.Entities;

namespace CoffeeShop;

public class ApplicationContext : DbContext
{
    public DbSet<Coffee> Coffees { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrdersToCoffees> OrdersToCoffees { get; set; }

    public ApplicationContext() => Database.EnsureCreated();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coffee>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Coffee>()
            .HasKey(c =>  c.Id);

        modelBuilder.Entity<Order>()
            .HasKey(o =>  o.Id);

        modelBuilder.Entity<OrdersToCoffees>()
            .HasKey(otc =>  otc.Id);

        modelBuilder.Entity<OrdersToCoffees>()
            .HasIndex(otc => new { otc.OrderId, otc.CoffeeId });

        modelBuilder.Entity<OrdersToCoffees>()
            .HasOne(otc => otc.Order)
            .WithMany(o => o.OrdersToCoffees)
            .HasForeignKey(otc => otc.OrderId);
        
        modelBuilder.Entity<OrdersToCoffees>()
            .HasOne(otc => otc.Coffee)
            .WithMany(c => c.OrdersToCoffees)
            .HasForeignKey(otc => otc.CoffeeId);

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
    public async Task AddOrderAsync(ICollection<string> coffeeNames)
    {
        var tempOrder = new Order();
        foreach (var coffeeName in coffeeNames)
        {
            var tempCoffee = await Coffees.FirstOrDefaultAsync(c => c.Name == coffeeName);
            if (tempCoffee == null)
                throw new ArgumentException(coffeeName + "not found");
            
            
            await OrdersToCoffees.AddAsync(new OrdersToCoffees
            {
                Coffee = tempCoffee,
                Order = tempOrder
            });
        }

        await Orders.AddAsync(tempOrder);

        await SaveChangesAsync();
        
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await Orders
            .Include(o => o.OrdersToCoffees)
            .ThenInclude(otc => otc.Coffee)
            .ToListAsync();
    }

    public IEnumerable<Order> GetOrdersAfter(DateTime dateTime)
    {
       return Orders
            .Where(o => o.Time > dateTime)
            .Include(o => o.OrdersToCoffees)
            .ThenInclude(otc => otc.Coffee);
    }

    public async Task DeleteOrder(string orderId)
    {
        var tempOrder = await Orders.FirstOrDefaultAsync(o => o.Id == Guid.Parse(orderId));
        if (tempOrder == null)
            throw new ArgumentException("Not found order with order id:" + orderId);
        Orders.Remove(tempOrder);
    }

}