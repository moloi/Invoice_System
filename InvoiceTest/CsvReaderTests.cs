using InvoiceSystem.Utilities;
using InvoiceSystem.Models;
using Xunit;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class CsvReaderTests
{
    private readonly CsvReader _csvReader;

    public CsvReaderTests()
    {
        _csvReader = new CsvReader();
    }

    [Fact]
    public void ReadCsv_ValidFile_ReturnsInvoices()
    {
        // Arrange
        string tempFilePath = Path.GetTempFileName();
        try
        {
            var csvContent = new StringBuilder();
            csvContent.AppendLine("InvoiceNumber,InvoiceDate,InvoiceLines");
            csvContent.AppendLine("INV001,2023-10-01,Line1;Line2");
            csvContent.AppendLine("INV002,2023-10-02,Line3;Line4");

            File.WriteAllText(tempFilePath, csvContent.ToString());

            // Act
            var invoices = _csvReader.ReadCsv(tempFilePath);

            // Assert
            Assert.NotEmpty(invoices);
            // Additional assertions to check invoice properties
            Assert.All(invoices, invoice =>
            {
                Assert.False(string.IsNullOrWhiteSpace(invoice.InvoiceNumber));
                Assert.NotEqual(default(DateTime), invoice.InvoiceDate);
                Assert.NotEmpty(invoice.InvoiceLines);
            });
        }
        finally
        {
            // Clean up the temporary file
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [Fact]
    public void ReadCsv_InvalidFile_LogsError()
    {
        string invalidCsvContent = "Header1,Header2\nValue1,Value2\nMalformedLineWithoutComma";
        string tempFilePath = Path.GetTempFileName() + ".csv";
        File.WriteAllText(tempFilePath, invalidCsvContent);

        string logFilePath = "error_log.txt";
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        // Act
        var invoices = _csvReader.ReadCsv(tempFilePath);

        // Assert
        // Check that the error log contains the expected error messages
        string logContents = File.ReadAllText(logFilePath);
        Assert.Contains("Malformed line", logContents);

        // Clean up
        File.Delete(tempFilePath);
    }
}