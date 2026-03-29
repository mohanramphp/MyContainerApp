namespace MyContainerApp.Application.DTOs;

/// <summary>
/// DTO for updating an existing pizza.
/// </summary>
public class UpdatePizzaRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
