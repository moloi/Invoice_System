using InvoiceSystem.Interface;
using InvoiceSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

public class InvoiceService
{
    private readonly ICsvReader? _csvReader;

    private static readonly string logFilePath = "error_log.txt"; // Path for logging errors

    public InvoiceService(ICsvReader csvReader)
    {
        _csvReader = csvReader;
    }

    public static void ImportInvoices(string csvFilePath, DbContextOptions<InvoiceContext> options, ICsvReader csvReader)
    {
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var invoices = csvReader.ReadCsv(csvFilePath);
            using (var context = new InvoiceContext(options))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var invoice in invoices)
                        {
                            if (!context.InvoiceHeaders.Any(h => h.InvoiceNumber == invoice.InvoiceNumber))
                            {
                                context.InvoiceHeaders.Add(invoice);
                                LogInfo($"Imported Invoice: {invoice.InvoiceNumber}");

                                var totalQuantity = invoice.InvoiceLines.Sum(line => line.Quantity);
                                LogInfo($"Total Quantity for Invoice {invoice.InvoiceNumber}: {totalQuantity}");
                            }
                            else
                            {
                                LogError($"Invoice {invoice.InvoiceNumber} already exists. Skipping import.");
                            }
                        }

                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogError($"Error during import transaction: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }

            CheckInvoiceTotals(options);
        }
        catch (FileNotFoundException ex)
        {
            LogError($"Error: The file '{csvFilePath}' was not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            LogError($"Error importing invoices: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static void LogError(string message)
    {
        // Log error messages to a file
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
    }

    private static void CheckInvoiceTotals(DbContextOptions<InvoiceContext> options)
    {
        using (var context = new InvoiceContext(options))
        {
            var totalInvoiceLines = context.InvoiceLines.Sum(line => line.Quantity * line.UnitSellingPriceExVAT);
            var totalInvoiceHeaders = context.InvoiceHeaders.Sum(header => header.InvoiceTotal);

            LogInfo($"Total from InvoiceLines: {totalInvoiceLines}");
            LogInfo($"Total from InvoiceHeaders: {totalInvoiceHeaders}");

            if (totalInvoiceLines == totalInvoiceHeaders)
            {
                LogInfo("Invoice totals match.");
            }
            else
            {
                LogError("Invoice totals do not match.");
            }
        }
    }

    private static void LogInfo(string message)
    {
        // Log informational messages to a file
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"{DateTime.Now}: INFO: {message}");
        }
    }
}