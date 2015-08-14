using System.Collections.Generic;

namespace BikeDistributor
{
    public class Bike : IItem
    {
        public Bike(string brand, string model, int price)
        {
            Brand = brand;
            Model = model;
            Price = price;
        }

        public string Brand { get; private set; }

        public string Model { get; private set; }
        
        public decimal Price { get; set; }
    }
}