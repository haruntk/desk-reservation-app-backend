using AutoMapper;
using DeskReservationApp.Application.DTOs.Desk;
using DeskReservationApp.Application.DTOs.Floor;
using DeskReservationApp.Application.DTOs.Reservation;
using DeskReservationApp.Application.DTOs.User;
using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Application.Mappings
{
    /// <summary>
    /// AutoMapper profiles for entity-DTO mappings
    /// </summary>
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // User mappings
            CreateMap<User, UserDTO>().ReverseMap();

            // Floor mappings
            CreateMap<Floor, FloorDTO>().ReverseMap();
            CreateMap<CreateFloorRequestDTO, Floor>();
            CreateMap<UpdateFloorRequestDTO, Floor>();
            CreateMap<Floor, FloorResponseDTO>()
                .ForMember(dest => dest.DeskCount, opt => opt.MapFrom(src => src.Desks.Count));

            // Desk mappings
            CreateMap<Desk, DeskDTO>().ReverseMap();
            CreateMap<CreateDeskRequestDTO, Desk>();
            CreateMap<UpdateDeskRequestDTO, Desk>();
            CreateMap<Desk, DeskResponseDTO>()
                .ForMember(dest => dest.FloorNumber, opt => opt.MapFrom(src => src.Floor.FloorNumber.ToString()));

            // Reservation mappings
            CreateMap<Reservation, ReservationDTO>().ReverseMap();
            CreateMap<CreateReservationRequestDTO, Reservation>();
            CreateMap<UpdateReservationRequestDTO, Reservation>();
            CreateMap<Reservation, ReservationResponseDTO>()
                .ForMember(dest => dest.DeskName, opt => opt.MapFrom(src => src.Desk.DeskName))
                .ForMember(dest => dest.FloorNumber, opt => opt.MapFrom(src => src.Desk.Floor.FloorNumber));
        }
    }
}
