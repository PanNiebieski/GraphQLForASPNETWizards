using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace _05RestAPIDocumentViews;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DocumentView> DocumentViews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Create a composite index on DocumentId and Date for efficient queries
        modelBuilder.Entity<DocumentView>()
            .HasIndex(dv => new { dv.DocumentId, dv.Date });
    }
}