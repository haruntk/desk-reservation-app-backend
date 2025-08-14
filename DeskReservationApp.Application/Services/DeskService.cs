using AutoMapper;
using DeskReservationApp.Application.DTOs.Desk;
using DeskReservationApp.Application.Exceptions;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Application.Services
{
    public class DeskService : IDeskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateDeskAsync(CreateDeskRequestDTO createDeskRequest)
        {
            var floor = await _unitOfWork.Floors.GetByIdAsync(createDeskRequest.FloorId);
            if (floor == null)
            {
                throw new BadRequestException($"Floor with id {createDeskRequest.FloorId} does not exist.");
            }

            var deskNameExists = await _unitOfWork.Desks.DeskNameExistsOnFloorAsync(createDeskRequest.DeskName, createDeskRequest.FloorId);
            if (deskNameExists)
            {
                throw new BadRequestException($"Desk with name '{createDeskRequest.DeskName}' already exists on this floor.");
            }

            var desk = _mapper.Map<Desk>(createDeskRequest);
            await _unitOfWork.Desks.AddAsync(desk);
            await _unitOfWork.SaveChangesAsync();
            return desk.DeskId;
        }

        public async Task DeleteDeskAsync(int deskId)
        {
            var desk = await _unitOfWork.Desks.GetByIdAsync(deskId);
            if (desk == null)
            {
                throw new NotFoundException(nameof(Desk), deskId);
            }

            _unitOfWork.Desks.Delete(desk);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<DeskResponseDTO>> GetAvailableDesksAsync(DeskAvailabilityRequestDTO availabilityRequest)
        {
            var desks = await _unitOfWork.Desks.GetAvailableDesksAsync(availabilityRequest.StartTime, availabilityRequest.EndTime);
            var deskDtos = _mapper.Map<IEnumerable<DeskResponseDTO>>(desks);

            foreach (var deskDto in deskDtos)
            {
                var desk = desks.First(d => d.DeskId == deskDto.DeskId);
                deskDto.IsAvailable = !desk.Reservations.Any(r => r.Status == "Active" && r.StartTime <= DateTime.UtcNow && r.EndTime > DateTime.UtcNow);
                var nextReservation = desk.Reservations.Where(r => r.Status == "Active" && r.StartTime > DateTime.UtcNow).OrderBy(r => r.StartTime).FirstOrDefault();
                deskDto.NextReservationStart = nextReservation?.StartTime;
            }

            return deskDtos;
        }

        public async Task<IEnumerable<DeskResponseDTO>> GetAllDesksAsync()
        {
            var desks = await _unitOfWork.Desks.GetAllAsync();
            return _mapper.Map<IEnumerable<DeskResponseDTO>>(desks);
        }

        public async Task<DeskDTO> GetDeskByIdAsync(int deskId)
        {
            var desk = await _unitOfWork.Desks.GetByIdAsync(deskId);
            if (desk == null)
            {
                throw new NotFoundException(nameof(Desk), deskId);
            }
            return _mapper.Map<DeskDTO>(desk);
        }

        public async Task<IEnumerable<DeskResponseDTO>> GetDesksByFloorIdAsync(int floorId)
        {
            var desks = await _unitOfWork.Desks.GetDesksByFloorIdAsync(floorId);
            return _mapper.Map<IEnumerable<DeskResponseDTO>>(desks);
        }

        public async Task UpdateDeskAsync(int deskId, UpdateDeskRequestDTO updateDeskRequest)
        {
            var desk = await _unitOfWork.Desks.GetByIdAsync(deskId);
            if (desk == null)
            {
                throw new NotFoundException(nameof(Desk), deskId);
            }

            if (desk.FloorId != updateDeskRequest.FloorId)
            {
                var floor = await _unitOfWork.Floors.GetByIdAsync(updateDeskRequest.FloorId);
                if (floor == null)
                {
                    throw new BadRequestException($"Floor with id {updateDeskRequest.FloorId} does not exist.");
                }
            }
            
            var deskNameExists = await _unitOfWork.Desks.DeskNameExistsOnFloorAsync(updateDeskRequest.DeskName, updateDeskRequest.FloorId, deskId);
            if (deskNameExists)
            {
                throw new BadRequestException($"Desk with name '{updateDeskRequest.DeskName}' already exists on this floor.");
            }

            _mapper.Map(updateDeskRequest, desk);
            _unitOfWork.Desks.Update(desk);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
