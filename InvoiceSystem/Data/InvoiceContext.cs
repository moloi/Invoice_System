using InvoiceSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class InvoiceContext : DbContext
{
    public InvoiceContext(DbContextOptions<InvoiceContext> options) : base(options) { }

    public DbSet<InvoiceHeader> InvoiceHeaders { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InvoiceHeader>()
                    .ToTable("InvoiceHeader")
                    .HasKey(i => i.InvoiceId);

        modelBuilder.Entity<InvoiceLine>()
                    .ToTable("InvoiceLines")
                    .HasKey(l => l.LineId);

        modelBuilder.Entity<InvoiceLine>()
                    .HasOne(line => line.InvoiceHeader)
                    .WithMany(header => header.InvoiceLines) 
                    .HasForeignKey(line => line.InvoiceId);
    }
}
