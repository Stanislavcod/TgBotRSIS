using AutoMapper;
using Bot.Common.DtoModels;
using Bot.Model.Models;

namespace Bot.Common.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
