namespace MyContainerApp.Application.DTOs;

/// <summary>
/// DTO for creating a new pizza.
/// </summary>
public class CreatePizzaRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
