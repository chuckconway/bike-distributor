namespace BikeDistributor
{
    public interface IItem
    {
        string Brand { get; }

        string Model { get; }

        decimal Price { get; set; }
    }
}