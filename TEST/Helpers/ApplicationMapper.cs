using AutoMapper;
using TEST.Data;
using TEST.Models.User;

namespace TEST.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {

            CreateMap<RegisterDto, User>()
               .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name)
            ).ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email)
            ).ForMember(
                dest => dest.Password,
                opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password))
                );

            CreateMap<LoginDto, User>()
                .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email)
            ).ForMember(
                dest => dest.Password,
                opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password))
                );

            CreateMap<User, UserDto>()
                .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email));
        }
    }
}
