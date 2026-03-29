using AutoMapper;
using MyContainerApp.Domain.Aggregates.Pizza;
using MyContainerApp.Application.DTOs;

namespace MyContainerApp.Application.Mappings;

/// <summary>
/// AutoMapper profile for Pizza entity to DTO mappings.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Pizza, PizzaResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value));

        CreateMap<CreatePizzaRequest, Pizza>()
            .ConstructUsing((src, ctx) =>
            {
                var id = new MyContainerApp.Domain.ValueObjects.PizzaId(0);
                return new Pizza(id, src.Name, src.Price, src.Description);
            });
    }
}
