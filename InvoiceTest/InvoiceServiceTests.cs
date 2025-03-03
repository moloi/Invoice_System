using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InvoiceSystem.Interface;

namespace InvoiceTest
{
    public class InvoiceServiceTests
    {
        [Fact]
        public void ImportInvoices_ValidData_SavesToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create a mock CSV file content
            string csvFilePath = "path_to_valid_csv_file.csv";
            var invoices = new List<InvoiceHeader>
            {
                new InvoiceHeader
                {
                    InvoiceNumber = "INV001",
                    InvoiceTotal = 100.00m,
                    InvoiceLines = new List<InvoiceLine>
                    {
                        new InvoiceLine { Quantity = 1, UnitSellingPriceExVAT = 100.00m }
                    }
                },
                new InvoiceHeader
                {
                    InvoiceNumber = "INV002",
                    InvoiceTotal = 200.00m,
                    InvoiceLines = new List<InvoiceLine>
                    {
                        new InvoiceLine { Quantity = 2, UnitSellingPriceExVAT = 100.00m }
                    }
                }
            };

            // Create a mock CsvReader
            var mockCsvReader = new MockCsvReader(invoices);

            // Act
            InvoiceService.ImportInvoices(csvFilePath, options, mockCsvReader);

            // Assert
            using (var context = new InvoiceContext(options))
            {
                var savedInvoices = context.InvoiceHeaders.ToList();
                Assert.NotEmpty(savedInvoices);
                Assert.Equal(2, savedInvoices.Count);
                Assert.Contains(savedInvoices, h => h.InvoiceNumber == "INV001");
                Assert.Contains(savedInvoices, h => h.InvoiceNumber == "INV002");
            }
        }
    }

    // Mock classes for InvoiceHeader and InvoiceLine
    public class InvoiceHeader
    {
        public string InvoiceNumber { get; set; }
        public decimal InvoiceTotal { get; set; }
        public List<InvoiceLine> InvoiceLines { get; set; }
    }

    public class InvoiceLine
    {
        public int Quantity { get; set; }
        public decimal UnitSellingPriceExVAT { get; set; }
    }

    // Mock CsvReader class
    public class MockCsvReader : ICsvReader
    {
        private readonly List<InvoiceHeader> _invoices;

        public MockCsvReader(List<InvoiceHeader> invoices)
        {
            _invoices = invoices;
        }

        public List<InvoiceHeader> ReadCsv(string path)
        {
            return _invoices;
        }

        List<InvoiceSystem.Models.InvoiceHeader> ICsvReader.ReadCsv(string path)
        {
            throw new NotImplementedException();
        }
    }
}