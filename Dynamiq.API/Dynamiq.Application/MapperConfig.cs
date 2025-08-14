using AutoMapper;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.ValueObject;

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
                config.CreateMap<Cart, CartDto>().ReverseMap();
                config.CreateMap<CartItem, CartItemDto>().ReverseMap();
            });
        }
    }
}
