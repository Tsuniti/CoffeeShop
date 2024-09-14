namespace CoffeeShop.Entities;

public class Coffee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public double Price { get; set; }

    public ICollection<Order> Orders = new List<Order>();
}