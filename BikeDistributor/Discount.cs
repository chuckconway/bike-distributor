using System;

namespace BikeDistributor
{
    public class Discount
    {
        public Func<Line, bool> Condition { get; set; }
        
        public string Message { get; set; }

        public decimal? Percentage { get; set; }
        
        public decimal? Fixed { get; set; } 
    }
}