using Microsoft.EntityFrameworkCore;
using MyContainerApp.Domain.Aggregates.Pizza;
using MyContainerApp.Domain.Repositories;
using MyContainerApp.Domain.ValueObjects;

namespace MyContainerApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of the Pizza repository.
/// Provides data access operations for Pizza entities using in-memory database.
/// </summary>
public class PizzaRepository : IPizzaRepository
{
    private readonly ApplicationDbContext _context;

    public PizzaRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(Pizza pizza, CancellationToken cancellationToken = default)
    {
        if (pizza == null)
            throw new ArgumentNullException(nameof(pizza));

        await _context.Pizzas.AddAsync(pizza, cancellationToken);
    }

    public async Task<Pizza?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Pizza ID must be a positive integer.", nameof(id));

        return await _context.Pizzas
            .FirstOrDefaultAsync(p => p.Id.Value == id, cancellationToken);
    }

    public async Task<IEnumerable<Pizza>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pizzas
            .OrderBy(p => p.Id.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Pizza pizza, CancellationToken cancellationToken = default)
    {
        if (pizza == null)
            throw new ArgumentNullException(nameof(pizza));

        var existing = await GetByIdAsync(pizza.Id.Value, cancellationToken);
        
        if (existing == null)
            throw new InvalidOperationException($"Pizza with ID {pizza.Id.Value} not found.");

        _context.Pizzas.Update(pizza);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Pizza ID must be a positive integer.", nameof(id));

        var pizza = await GetByIdAsync(id, cancellationToken);
        
        if (pizza == null)
            throw new InvalidOperationException($"Pizza with ID {id} not found.");

        _context.Pizzas.Remove(pizza);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
