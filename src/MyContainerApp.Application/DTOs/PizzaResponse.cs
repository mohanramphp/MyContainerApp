namespace MyContainerApp.Application.DTOs;

/// <summary>
/// DTO for returning pizza data in API responses.
/// </summary>
public class PizzaResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
