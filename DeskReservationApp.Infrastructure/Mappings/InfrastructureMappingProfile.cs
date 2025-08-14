using AutoMapper;
using DeskReservationApp.Application.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace DeskReservationApp.Infrastructure.Mappings
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<IdentityUser, UserDTO>().ReverseMap();
        }
    }
}
