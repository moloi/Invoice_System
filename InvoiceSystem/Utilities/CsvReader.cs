using InvoiceSystem.Interface;
using InvoiceSystem.Models;
using System.Globalization;

namespace InvoiceSystem.Utilities
{
    public class CsvReader : ICsvReader
    {
        private static readonly string logFilePath = "error_log.txt"; // Path for logging errors

        public List<InvoiceHeader> ReadCsv(string filePath)
        {
            var invoices = new List<InvoiceHeader>();
            var lines = File.ReadAllLines(filePath);

            // Start reading from the second line (skipping header)
            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split(',');

                // Check for the minimum number of columns
                if (columns.Length < 7)
                {
                    LogError($"Malformed line {i + 1}: Expected at least 7 columns.");
                    continue;
                }

                var invoiceNumber = columns[0].Trim();

                // Parse the invoice date
                if (!DateTime.TryParseExact(columns[1].Trim(), "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime invoiceDate))
                {
                    LogError($"Malformed date in line {i + 1}: {columns[1]}");
                    continue;
                }

                var address = string.IsNullOrWhiteSpace(columns[2]) ? "N/A" : columns[2].Trim();

                // Parse invoice total, quantity, and unit selling price
                if (!TryParseDecimal(columns[3].Trim(), out decimal invoiceTotal) ||
                    !TryParseInt(columns[5].Trim(), out int quantity) ||
                    !TryParseDecimal(columns[6].Trim(), out decimal unitSellingPriceExVAT))
                {
                    LogError($"Invalid data in line {i + 1}. Skipping line.");
                    continue;
                }

                var lineDescription = columns[4].Trim();

                // Check if the invoice already exists
                var invoice = invoices.FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);
                if (invoice == null)
                {
                    invoice = new InvoiceHeader
                    {
                        InvoiceNumber = invoiceNumber,
                        InvoiceDate = invoiceDate,
                        Address = address,
                        InvoiceTotal = (double)invoiceTotal,
                        InvoiceLines = new List<InvoiceLine>()
                    };
                    invoices.Add(invoice);
                }

                // Check for duplicate invoice lines
                if (!invoice.InvoiceLines.Any(line => line.Description == lineDescription && line.Quantity == quantity && line.UnitSellingPriceExVAT == (double)unitSellingPriceExVAT))
                {
                    invoice.InvoiceLines.Add(new InvoiceLine
                    {
                        Description = lineDescription,
                        Quantity = quantity,
                        UnitSellingPriceExVAT = (double)unitSellingPriceExVAT,
                        InvoiceId = invoice.InvoiceId,
                        InvoiceNumber = invoice.InvoiceNumber
                    });
                }
                else
                {
                    LogError($"Duplicate line for Invoice {invoice.InvoiceNumber} found. Skipping line.");
                }
            }

            return invoices;
        }

        private void LogError(string message)
        {
            // Log error messages to a file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        private bool TryParseDecimal(string value, out decimal result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                LogError("Value is null or empty.");
                return false;
            }

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return true;
            }

            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
            {
                if (doubleValue >= (double)decimal.MinValue && doubleValue <= (double)decimal.MaxValue)
                {
                    result = Convert.ToDecimal(doubleValue);
                    return true;
                }
            }

            LogError($"Failed to parse '{value}' as decimal.");
            return false;
        }

        private bool TryParseInt(string value, out int result)
        {
            return int.TryParse(value, out result);
        }
    }
}