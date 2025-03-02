using AutoMapper;
using Gauniv.WebServer.Data;
using static Gauniv.WebServer.Data.Game;

namespace Gauniv.WebServer.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Rajouter autant de ligne ici que vous avez de mapping Model <-> DTO
            // https://docs.automapper.org/en/latest/
            CreateMap<Game, GameDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList()));

            // Mapping de Category vers CategoryDto
            CreateMap<Game.Category, CategoryDto>();

            // Mappage de Category vers CategoryDto (directement depuis la classe Category)
            CreateMap<Category, CategoryDto>();


        }
    }
}
