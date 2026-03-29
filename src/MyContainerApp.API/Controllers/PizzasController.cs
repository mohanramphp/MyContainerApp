using MyContainerApp.Application.DTOs;
using MyContainerApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MyContainerApp.API.Controllers;

/// <summary>
/// Pizza API controller that exposes CRUD operations for pizzas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
    private readonly IPizzaApplicationService _pizzaApplicationService;
    private readonly ILogger<PizzasController> _logger;

    public PizzasController(IPizzaApplicationService pizzaApplicationService, ILogger<PizzasController> logger)
    {
        _pizzaApplicationService = pizzaApplicationService ?? throw new ArgumentNullException(nameof(pizzaApplicationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new pizza.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PizzaResponse>> CreatePizza(CreatePizzaRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new pizza: {PizzaName}", request.Name);
            var result = await _pizzaApplicationService.CreatePizzaAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetPizza), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error while creating pizza: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a pizza by its identifier.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PizzaResponse>> GetPizza(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pizza with ID: {PizzaId}", id);
            var result = await _pizzaApplicationService.GetPizzaAsync(id, cancellationToken);
            
            if (result == null)
            {
                _logger.LogWarning("Pizza with ID {PizzaId} not found", id);
                return NotFound(new { error = $"Pizza with ID {id} not found" });
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error while retrieving pizza: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all pizzas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PizzaResponse>>> GetAllPizzas(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all pizzas");
            var result = await _pizzaApplicationService.GetAllPizzasAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving all pizzas: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving pizzas" });
        }
    }

    /// <summary>
    /// Updates an existing pizza.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PizzaResponse>> UpdatePizza(int id, UpdatePizzaRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating pizza with ID: {PizzaId}", id);
            var result = await _pizzaApplicationService.UpdatePizzaAsync(id, request, cancellationToken);
            
            if (result == null)
            {
                _logger.LogWarning("Pizza with ID {PizzaId} not found for update", id);
                return NotFound(new { error = $"Pizza with ID {id} not found" });
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error while updating pizza: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a pizza by its identifier.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePizza(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting pizza with ID: {PizzaId}", id);
            var result = await _pizzaApplicationService.DeletePizzaAsync(id, cancellationToken);
            
            if (!result)
            {
                _logger.LogWarning("Pizza with ID {PizzaId} not found for deletion", id);
                return NotFound(new { error = $"Pizza with ID {id} not found" });
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error while deleting pizza: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all environment variables for verification (from ConfigMap and environment).
    /// This endpoint verifies that the application is reading configuration from the ConfigMap
    /// and environment variables, not just appsettings.json
    /// </summary>
    [HttpGet("config/environment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<object> GetEnvironmentVariables()
    {
        _logger.LogInformation("Retrieving environment variables configuration");

        var envVars = new
        {
            // Environment variables from ConfigMap
            LogLevel = $"From ConfigMap: {Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Not set"}",
            ApiName = $"From ConfigMap: {Environment.GetEnvironmentVariable("API_NAME") ?? "Not set"}",
            ApiVersion = $"From ConfigMap: {Environment.GetEnvironmentVariable("API_VERSION") ?? "Not set"}",
            ApiEnvironment = $"From ConfigMap: {Environment.GetEnvironmentVariable("API_ENVIRONMENT") ?? "Not set"}",
            
            // ASP.NET Core environment variables
            AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Not set",
            AspNetCoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "Not set",
            
            // Timestamp
            RetrievedAt = DateTime.UtcNow,
            
            // Source note
            Source = "These values are read from environment variables (ConfigMap injection), NOT from appsettings.json"
        };

        return Ok(envVars);
    }
}

