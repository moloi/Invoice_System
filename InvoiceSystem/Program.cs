using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using InvoiceSystem.Interface;
using InvoiceSystem.Utilities;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var services = new ServiceCollection();

        services.AddScoped<ICsvReader, CsvReader>();
        services.AddScoped<InvoiceService>();

        var connectionString = configuration.GetConnectionString("SqlServerConnection");

        var optionsBuilder = new DbContextOptionsBuilder<InvoiceContext>();
        optionsBuilder.UseSqlServer(connectionString);

        using (var context = new InvoiceContext(optionsBuilder.Options))
        {
            context.Database.EnsureCreated();
        }


        string csvFilePath = @"C:\Users\ITQ_DEVELOPER\Desktop\data.csv";

        try
        {
            ICsvReader csvReader = new CsvReader();

            InvoiceService.ImportInvoices(csvFilePath, optionsBuilder.Options, csvReader);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: The file '{csvFilePath}' was not found.");
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}