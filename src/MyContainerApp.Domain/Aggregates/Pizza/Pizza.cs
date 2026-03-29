using MyContainerApp.Domain.ValueObjects;

namespace MyContainerApp.Domain.Aggregates.Pizza;

/// <summary>
/// Pizza Aggregate Root representing a pizza product in the domain.
/// </summary>
public class Pizza
{
    public PizzaId Id { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string Description { get; private set; } = null!;

    // EF Core requires parameterless constructor for shadow properties
    private Pizza() { }

    /// <summary>
    /// Creates a new Pizza instance.
    /// </summary>
    public Pizza(PizzaId id, string name, decimal price, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Pizza name cannot be empty.", nameof(name));

        if (price < 0)
            throw new ArgumentException("Pizza price cannot be negative.", nameof(price));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Pizza description cannot be empty.", nameof(description));

        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name;
        Price = price;
        Description = description;
    }

    /// <summary>
    /// Updates the Pizza details.
    /// </summary>
    public void Update(string name, decimal price, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Pizza name cannot be empty.", nameof(name));

        if (price < 0)
            throw new ArgumentException("Pizza price cannot be negative.", nameof(price));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Pizza description cannot be empty.", nameof(description));

        Name = name;
        Price = price;
        Description = description;
    }
}
