using MyContainerApp.Domain.Aggregates.Pizza;

namespace MyContainerApp.Domain.Repositories;

/// <summary>
/// Repository interface for Pizza aggregate root operations.
/// Defines the contract for data access operations on Pizza entities.
/// </summary>
public interface IPizzaRepository
{
    /// <summary>
    /// Adds a new pizza to the repository.
    /// </summary>
    Task AddAsync(Pizza pizza, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a pizza by its identifier.
    /// </summary>
    Task<Pizza?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all pizzas from the repository.
    /// </summary>
    Task<IEnumerable<Pizza>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing pizza.
    /// </summary>
    Task UpdateAsync(Pizza pizza, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a pizza by its identifier.
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists all changes made in this repository to the underlying store.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
