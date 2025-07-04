using AutoMapper;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Mapping
{
    public class MapperConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<User, UserDto>().ReverseMap();
                config.CreateMap<RefreshToken, RefreshTokenDto>().ReverseMap();
                config.CreateMap<Product, ProductDto>().ReverseMap();
                config.CreateMap<PaymentHistory, PaymentHistoryDto>().ReverseMap();
            });
        }
    }
}
