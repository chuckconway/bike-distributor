using System;
using System.Collections.Generic;
using System.Globalization;
using BikeDistributor.Extensions;
using NUnit.Framework;

namespace BikeDistributor.Tests
{
    [TestFixture]
    public class OrdersTests
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

            twoHundredDollarDiscount = new Discount {Fixed = 200, Message = "Discount - $200", Condition = l => true};
            tenPercentDiscount = new Discount { Percentage = 0.10m, Message = "Discount - 10% Off", Condition = l => true };

            fiveItemDiscount = new Discount { Condition = s=> s.Quantity >= 5, Message = "5 IItem Discount - 5% off", Percentage = 0.05m};
            tenItemDiscount = new Discount { Condition = s => s.Quantity >= 10, Message = "10 IItem Discount - 10% off", Percentage = 0.1m };
            twentyItemDiscount = new Discount { Condition = s => s.Quantity >= 20, Message = "20 IItem Discount - 20% off", Percentage = 0.2m };

        }

        [Test]
        public void Order_GenerateReceipt_Successful()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 10));

            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.NotNull(receipt);
            Assert.AreEqual(10800m, receipt.Total, "Expecting total to equal $10800");
        }

        [Test]
        public void Order_GenerateReceiptWith10PercentDiscount_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 10, new List<Discount> { tenPercentDiscount }));

            var receipt = order.GenerateReceipt();
            WriteReceiptToConsole(receipt);

            Assert.AreEqual(9720m, receipt.Total);
        }

        [Test]
        public void Order_GenerateReceiptWithTwoHundredDollarDiscount_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 10, new List<Discount> { twoHundredDollarDiscount }));

            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.AreEqual(10584m, receipt.Total);
        }

        [Test]
        public void Order_GenerateReceiptWithMultipleDiscounts_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 10, new List<Discount> { twoHundredDollarDiscount, tenPercentDiscount }));

            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.AreEqual(9525.6m, receipt.Total);
        }

        [Test]
        public void Order_GenerateReceiptWithFiveItemDiscount_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 10, new List<Discount> { fiveItemDiscount }));

            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.AreEqual(10260m, receipt.Total);
        }

        [Test]
        public void Order_GenerateReceiptWithFiveItemDiscountWithLessThan5Items_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 4, new List<Discount> { fiveItemDiscount }));

            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.AreEqual(4320m, receipt.Total);
        }

        [Test]
        public void Order_GenerateReceiptWithTenItemDiscount_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 10, new List<Discount> { tenItemDiscount }));
            order.AddLine(new Line(mountainBike, 2));
            order.AddLine(new Line(bmxBike, 5, new List<Discount> { twoHundredDollarDiscount }));


            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.AreEqual(15436.44m, receipt.Total);
        }

        [Test]
        public void Order_GenerateReceiptWithTenItemDiscountWithLessThan10Items_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 9, new List<Discount> { tenItemDiscount }));
            order.AddLine(new Line(mountainBike, 2));
            order.AddLine(new Line(bmxBike, 5, new List<Discount> {twoHundredDollarDiscount}));

            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.AreEqual(15436.44m, receipt.Total);
        }

        [Test]
        public void Order_GenerateReceiptWithTwentyItemDiscount_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 20, new List<Discount> { twentyItemDiscount }));
            order.AddLine(new Line(mountainBike, 2));
            order.AddLine(new Line(bmxBike, 5, new List<Discount> { twoHundredDollarDiscount }));


            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.AreEqual(22996.44m, receipt.Total);
        }

        [Test]
        public void Order_GenerateReceiptWithTwentyItemDiscountWithLessThan20Items_Success()
        {
            var order = new Order(companyName, 0.08m);
            order.AddLine(new Line(roadBike, 19, new List<Discount> { twentyItemDiscount }));
            order.AddLine(new Line(mountainBike, 2));
            order.AddLine(new Line(bmxBike, 5, new List<Discount> { twoHundredDollarDiscount }));

            var receipt = order.GenerateReceipt();

            WriteReceiptToConsole(receipt);

            Assert.AreEqual(26236.44m, receipt.Total);
        }



        private static void WriteReceiptToConsole(Receipt receipt)
        {
            Console.WriteLine("Company Name: {0}", receipt.Company);
            Console.WriteLine("Invoice: {0}", receipt.InvoiceNumber);
            Console.WriteLine("Date: {0}", receipt.Date.ToString(CultureInfo.InvariantCulture));
            Console.WriteLine(Environment.NewLine);

            receipt.Items.ForEach(s => Console.WriteLine(string.Format("Qty: {0}, Description: {1}, Total: {2}", s.Quantity, s.Description, s.Total)));

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("SubTotal: {0}", receipt.SubTotal);
            Console.WriteLine("Tax Rate: {0}", receipt.TaxRate.ToPercentageStringFormat());
            Console.WriteLine("Tax: {0}", receipt.Tax);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Total: {0}", receipt.Total);
        }
    }
}
