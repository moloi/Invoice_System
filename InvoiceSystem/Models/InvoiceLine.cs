using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceSystem.Models
{
    public class InvoiceLine
    {
        public int LineId { get; set; }
        public int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? Description { get; set; }
        public double Quantity { get; set; }  
        public double UnitSellingPriceExVAT { get; set; } 
        public InvoiceHeader InvoiceHeader { get; set; }
    }

}
