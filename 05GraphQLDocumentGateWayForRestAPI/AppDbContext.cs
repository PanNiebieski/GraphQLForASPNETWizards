using _05GraphQLDocumentGateWayForRestAPI.Models;
using Microsoft.EntityFrameworkCore;
using Tag = _05GraphQLDocumentGateWayForRestAPI.Models.Tag;

namespace _05GraphQLDocumentGateWayForRestAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Metadata> Metadata => Set<Metadata>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure many-to-many relationship between Documents and Tags
        modelBuilder.Entity<Document>()
            .HasMany(d => d.Tags)
            .WithMany(t => t.Documents)
            .UsingEntity(j => j.ToTable("DocumentTags"));

        // Configure one-to-many relationship between Document and Metadata
        modelBuilder.Entity<Document>()
            .HasMany(d => d.Metadata)
            .WithOne(m => m.Document)
            .HasForeignKey(m => m.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}