using Microsoft.EntityFrameworkCore;
using MyContainerApp.Domain.Aggregates.Pizza;
using MyContainerApp.Domain.ValueObjects;

namespace MyContainerApp.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for the Pizza application.
/// Configured to use in-memory database provider.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public DbSet<Pizza> Pizzas { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Pizza entity
        var pizzaEntityTypeBuilder = modelBuilder.Entity<Pizza>();

        pizzaEntityTypeBuilder.HasKey(p => p.Id);

        pizzaEntityTypeBuilder.Property(p => p.Id)
            .HasConversion(
                pizzaId => pizzaId.Value,
                value => new PizzaId(value))
            .IsRequired();

        pizzaEntityTypeBuilder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        pizzaEntityTypeBuilder.Property(p => p.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        pizzaEntityTypeBuilder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        // Seed initial data (optional)
        // You can add seed data here if needed
    }
}
