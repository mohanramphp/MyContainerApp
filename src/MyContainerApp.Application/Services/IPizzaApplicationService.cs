using MyContainerApp.Application.DTOs;

namespace MyContainerApp.Application.Services;

/// <summary>
/// Interface for pizza application service that handles CRUD operations.
/// </summary>
public interface IPizzaApplicationService
{
    /// <summary>
    /// Creates a new pizza.
    /// </summary>
    Task<PizzaResponse> CreatePizzaAsync(CreatePizzaRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a pizza by its identifier.
    /// </summary>
    Task<PizzaResponse?> GetPizzaAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all pizzas.
    /// </summary>
    Task<IEnumerable<PizzaResponse>> GetAllPizzasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing pizza.
    /// </summary>
    Task<PizzaResponse?> UpdatePizzaAsync(int id, UpdatePizzaRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a pizza by its identifier.
    /// </summary>
    Task<bool> DeletePizzaAsync(int id, CancellationToken cancellationToken = default);
}
