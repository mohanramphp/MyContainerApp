namespace MyContainerApp.Domain.ValueObjects;

/// <summary>
/// Value Object representing a unique Pizza identifier.
/// </summary>
public class PizzaId : IEquatable<PizzaId>
{
    public int Value { get; }

    public PizzaId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("PizzaId must be a positive integer.", nameof(value));

        Value = value;
    }

    public bool Equals(PizzaId? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PizzaId);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(PizzaId left, PizzaId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PizzaId left, PizzaId right)
    {
        return !left.Equals(right);
    }
}
