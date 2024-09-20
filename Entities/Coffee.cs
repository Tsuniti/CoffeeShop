namespace CoffeeShop.Entities;

public class Coffee
{
    public Guid Id = Guid.NewGuid() ;
    public string Name { get; set; }
    public double Price { get; set; }

    public ICollection<OrdersToCoffees> OrdersToCoffees = new List<OrdersToCoffees>();
}