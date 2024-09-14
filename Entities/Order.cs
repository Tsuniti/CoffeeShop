namespace CoffeeShop.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Time { get; set; } = DateTime.Now;

    public Guid CoffeeId { get; set; }
    public Coffee Coffee { get; set; }
}