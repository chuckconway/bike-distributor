using System.Collections.Generic;

namespace BikeDistributor
{
    public class Line
    {

        public IItem Item { get; private set; }

        public int Quantity { get; private set; }

        public IList<Discount> Discounts { get; private set; }

        public Line(IItem item, int quantity, IList<Discount> discounts)
        {
            Item = item;
            Quantity = quantity;
            Discounts = discounts;
        }

        public Line(IItem item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
