using AutoMapper;
using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Entities;

namespace Dynamiq.Application
{
    public class MapperConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<User, UserDto>().ReverseMap();
                config.CreateMap<Product, ProductDto>().ReverseMap();
            });
        }
    }
}
