using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BikeDistributor.Tests
{
    public class InvoiceTests
    {
        private Bike roadBike;
        private Bike mountainBike;
        private Bike bmxBike;

        const string companyName = "ACME Inc.";

        private Discount tenPercentDiscount;
        private Discount twoHundredDollarDiscount;
        private Discount fiveItemDiscount;
        private Discount tenItemDiscount;
        private Discount twentyItemDiscount;


        [SetUp]
        public void Setup()
        {
            roadBike = new Bike("Speedster", "2000", 1000);
            mountainBike = new Bike("Cannondale", "F3", 1249);
            bmxBike = new Bike("Tricketer", "457", 599);

            twoHundredDollarDiscount = new Discount { Fixed = 200, Message = "Discount - $200", Condition = l => true };
            tenPercentDiscount = new Discount { Percentage = 0.10m, Message = "Discount - 10% Off", Condition = l => true };

            fiveItemDiscount = new Discount { Condition = s => s.Quantity >= 5, Message = "5 IItem Discount - 5% off", Percentage = 0.05m };
            tenItemDiscount = new Discount { Condition = s => s.Quantity >= 10, Message = "10 IItem Discount - 10% off", Percentage = 0.1m };
            twentyItemDiscount = new Discount { Condition = s => s.Quantity >= 20, Message = "20 IItem Discount - 20% off", Percentage = 0.2m };

        }

        [Test]
        public void Invoice_GenerateInvoice_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 19, new List<Discount> { twentyItemDiscount }));
            order.AddLine(new Line(mountainBike, 2));
            order.AddLine(new Line(bmxBike, 5, new List<Discount> { twoHundredDollarDiscount }));

            var invoice = order.GenerateInvoice();
            Console.Write(invoice);
        }

        [Test]
        public void Invoice_GenerateHtmlInvoice_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 19, new List<Discount> { twentyItemDiscount }));
            order.AddLine(new Line(mountainBike, 2));
            order.AddLine(new Line(bmxBike, 5, new List<Discount> { twoHundredDollarDiscount }));

            var invoice = order.GenerateHtmlInvoice();
            Console.Write(invoice);
        }
    }
}
