using AutoMapper;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Extension.DTOs;

namespace Dynamiq.API
{
    public class MapperConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<User, UserDto>().ReverseMap();

            });
        }
    }
}
