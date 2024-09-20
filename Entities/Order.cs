namespace CoffeeShop.Entities;

public class Order
{
    public Guid Id  = Guid.NewGuid();
    public DateTime Time { get; set; } = DateTime.Now;

    public ICollection<OrdersToCoffees> OrdersToCoffees { get; set; } = new List<OrdersToCoffees>();

}