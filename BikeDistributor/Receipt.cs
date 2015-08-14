using System;
using System.Collections.Generic;
using System.Linq;

namespace BikeDistributor
{
    public class Receipt
    {

        public Receipt(decimal taxRate)
        {
            TaxRate = taxRate;
            Items = new List<ReciptLineItem>();
        }

        public List<ReciptLineItem> Items { get; private set; }
        
        public string Company { get; set; }

        public DateTime Date { get; set; }

        public int InvoiceNumber { get; set; }
        
        public decimal SubTotal { get; set; }
        
        public decimal TaxRate { get; set; }
        
        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public decimal CalculateSubTotal()
        {
            return this.Items.Sum(s => s.Total);
        }

        public decimal CalculateTax()
        {
            return SubTotal * TaxRate;
        }

        public void ReconcileTotal()
        {
            SubTotal = CalculateSubTotal();
            Tax = CalculateTax();
            Total = SubTotal + Tax;
        }
    }
}