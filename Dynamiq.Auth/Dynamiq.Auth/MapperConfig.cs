using AutoMapper;
using Dynamiq.Auth.DAL.Models;
using Dynamiq.Auth.DTOs;

namespace Dynamiq.Auth
{
    public class MapperConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<User, UserDto>()
                    .ForMember(dest => dest.Password, opt => opt.Ignore());

                config.CreateMap<UserDto, User>()
                    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            });
        }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
