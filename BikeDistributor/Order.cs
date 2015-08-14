using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BikeDistributor.Extensions;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Text;
using RazorEngineService = RazorEngine.Templating.RazorEngineService;
using RazorEngine.Templating;

namespace BikeDistributor
{
    public class Order
    {
        public string Company { get; private set; }
        private readonly decimal _taxRate;

        readonly IList<Line> _lines = new List<Line>(); 

        public Order(string company, decimal taxRate)
        {
            Company = company;
            _taxRate = taxRate;
        }

        public void AddLine(Line line)
        {
            _lines.Add(line);
        }

        public Receipt GenerateReceipt()
        {
            var recipt = new Receipt(_taxRate)
            {
                Company = Company,
                InvoiceNumber = new Random().Next(1000, 10000), //Just a randon invoice number, we'd use a primary key or a generated number for the invoice number
                Date = DateTime.UtcNow //in a real system we would convert this to the correct timezone.
            };
            
            ProcessLineItems(recipt);
            recipt.ReconcileTotal();

            return recipt;
        }

        public string GenerateInvoice()
        {
            var receipt = GenerateReceipt();
            return CompileTemplate(receipt, "BikeDistributor.InvoiceTemplates.InvoiceTemplate.cshtml", "InvoiceKey");
        }

        public string GenerateHtmlInvoice()
        {
            var receipt = GenerateReceipt();
            return CompileTemplate(receipt, "BikeDistributor.InvoiceTemplates.InvoiceHtmlTemplate.cshtml", "InvoiceHtmlKey");
        }

        private void ProcessLineItems(Receipt receipt)
        {
            foreach (var line in _lines)
            {
                var price = line.Item.Price;
                var qty = line.Quantity;
                var sub = (decimal) (price*qty);

                receipt.Items.Add(new ReciptLineItem
                {
                    Description = line.Name(),
                    Quantity = qty,
                    Total = sub
                });

                var discounts = ApplyDiscounts(line, sub);
                receipt.Items.AddRange(discounts);
            }
        }

        private static IEnumerable<ReciptLineItem> ApplyDiscounts(Line line, decimal lineSubTotal)
        {
            //This code allows for more multiple discounts applied to each line.
            var items = new List<ReciptLineItem>();
            var discounts = line.Discounts ?? new List<Discount>();

            foreach (var discount in discounts)
            {
                CheckDiscountConditions(line, lineSubTotal, discount, items);
            }

            return items;
        }

        private static void CheckDiscountConditions(Line line, decimal lineSubTotal, Discount discount, ICollection<ReciptLineItem> items)
        {
            var qualifiesForDiscount = discount.Condition(line);
            var item = new ReciptLineItem();

            //There may be other discounts applied to this line. We need to ensure that we don't stack the discounts. Discounts are compounding.
            var addedDiscounts = items.Sum(s => s.Total);

            if (qualifiesForDiscount)
            {
                //We don't want to have discount that is greater than total.
                if (discount.Fixed.HasValue && (discount.Fixed.Value) < lineSubTotal + addedDiscounts)
                {
                    item.Description = discount.Message;
                    item.Total = discount.Fixed.Value * -1;

                    items.Add(item);
                }

                if (discount.Percentage.HasValue)
                {
                    //Make sure that if other discounts are taken into account.
                    var discounted = ((lineSubTotal + addedDiscounts) * discount.Percentage.Value);

                    item.Description = discount.Message;
                    item.Total = discounted * -1;

                    items.Add(item);
                }
            }
        }

        private static string CompileTemplate(Receipt model, string resourceName, string templateKey)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var template = string.Empty;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                template = reader.ReadToEnd();
            }

            var config = new TemplateServiceConfiguration {EncodedStringFactory = new RawStringFactory()};
            Engine.Razor = RazorEngineService.Create(config);

            return Engine.Razor.RunCompile(template, templateKey, null, model);
        }
    }
}
