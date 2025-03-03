    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace InvoiceSystem.Models
    {
        public class InvoiceHeader
        {
            public int InvoiceId { get; set; }
            public string? InvoiceNumber { get; set; }
            public DateTime InvoiceDate { get; set; }
            public string? Address { get; set; }
            public double InvoiceTotal { get; set; }

            [NotMapped]
            public double InvoiceTotalDouble => InvoiceTotal;

            public ICollection<InvoiceLine>? InvoiceLines { get; set; } = new List<InvoiceLine>();
        }
    }
