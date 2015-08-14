using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeDistributor.Extensions
{
    public static class DecimalExtensions
    {

        public static string ToPercentageStringFormat(this decimal d)
        {
            //Formats the decimal to a percentage with one decimal place.
            return d.ToString("P1");
        }
    }
}
