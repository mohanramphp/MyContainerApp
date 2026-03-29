using AutoMapper;
using MyContainerApp.Domain.Repositories;
using MyContainerApp.Domain.Aggregates.Pizza;
using MyContainerApp.Domain.ValueObjects;
using MyContainerApp.Application.DTOs;

namespace MyContainerApp.Application.Services;

/// <summary>
/// Application service for managing pizza operations.
/// Orchestrates domain and repository operations to implement use cases.
/// </summary>
public class PizzaApplicationService : IPizzaApplicationService
{
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IMapper _mapper;

    public PizzaApplicationService(IPizzaRepository pizzaRepository, IMapper mapper)
    {
        _pizzaRepository = pizzaRepository ?? throw new ArgumentNullException(nameof(pizzaRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PizzaResponse> CreatePizzaAsync(CreatePizzaRequest request, CancellationToken cancellationToken = default)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Pizza name is required.", nameof(request.Name));

        if (request.Price < 0)
            throw new ArgumentException("Pizza price cannot be negative.", nameof(request.Price));

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Pizza description is required.", nameof(request.Description));

        // Get all existing pizzas to determine the next ID
        var existingPizzas = await _pizzaRepository.GetAllAsync(cancellationToken);
        var nextId = existingPizzas.Any() ? existingPizzas.Max(p => p.Id.Value) + 1 : 1;

        // Create domain entity
        var pizzaId = new PizzaId(nextId);
        var pizza = new Pizza(pizzaId, request.Name, request.Price, request.Description);

        // Persist
        await _pizzaRepository.AddAsync(pizza, cancellationToken);
        await _pizzaRepository.SaveChangesAsync(cancellationToken);

        // Return response
        return _mapper.Map<PizzaResponse>(pizza);
    }

    public async Task<PizzaResponse?> GetPizzaAsync(int id, CancellationToken cancellationToken = default)
    {
        var pizza = await _pizzaRepository.GetByIdAsync(id, cancellationToken);
        
        if (pizza is null)
            return null;

        return _mapper.Map<PizzaResponse>(pizza);
    }

    public async Task<IEnumerable<PizzaResponse>> GetAllPizzasAsync(CancellationToken cancellationToken = default)
    {
        var pizzas = await _pizzaRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PizzaResponse>>(pizzas);
    }

    public async Task<PizzaResponse?> UpdatePizzaAsync(int id, UpdatePizzaRequest request, CancellationToken cancellationToken = default)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Pizza name is required.", nameof(request.Name));

        if (request.Price < 0)
            throw new ArgumentException("Pizza price cannot be negative.", nameof(request.Price));

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Pizza description is required.", nameof(request.Description));

        // Get existing pizza
        var pizza = await _pizzaRepository.GetByIdAsync(id, cancellationToken);
        
        if (pizza is null)
            return null;

        // Update pizza
        pizza.Update(request.Name, request.Price, request.Description);

        // Persist
        await _pizzaRepository.UpdateAsync(pizza, cancellationToken);
        await _pizzaRepository.SaveChangesAsync(cancellationToken);

        // Return response
        return _mapper.Map<PizzaResponse>(pizza);
    }

    public async Task<bool> DeletePizzaAsync(int id, CancellationToken cancellationToken = default)
    {
        var pizza = await _pizzaRepository.GetByIdAsync(id, cancellationToken);
        
        if (pizza is null)
            return false;

        await _pizzaRepository.DeleteAsync(id, cancellationToken);
        await _pizzaRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
