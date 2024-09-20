namespace CoffeeShop.Entities;

public class OrdersToCoffees
{
    public Guid Id = Guid.NewGuid();
    public Guid OrderId;
    public Order Order;

    public Guid CoffeeId;
    public Coffee Coffee { get; set; }

}