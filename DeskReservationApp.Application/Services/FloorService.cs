using AutoMapper;
using DeskReservationApp.Application.DTOs.Floor;
using DeskReservationApp.Application.Exceptions;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Application.Services
{
    public class FloorService : IFloorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FloorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateFloorAsync(CreateFloorRequestDTO createFloorRequest)
        {
            var floorExists = await _unitOfWork.Floors.FloorNumberExistsAsync(createFloorRequest.FloorNumber);
            if (floorExists)
            {
                throw new BadRequestException($"Floor with number {createFloorRequest.FloorNumber} already exists.");
            }

            var floor = _mapper.Map<Floor>(createFloorRequest);
            await _unitOfWork.Floors.AddAsync(floor);
            await _unitOfWork.SaveChangesAsync();
            return floor.FloorId;
        }

        public async Task DeleteFloorAsync(int floorId)
        {
            var floor = await _unitOfWork.Floors.GetByIdAsync(floorId);
            if (floor == null)
            {
                throw new NotFoundException(nameof(Floor), floorId);
            }

            _unitOfWork.Floors.Delete(floor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<FloorResponseDTO>> GetAllFloorsAsync()
        {
            var floors = await _unitOfWork.Floors.GetAllAsync();
            return _mapper.Map<IEnumerable<FloorResponseDTO>>(floors);
        }

        public async Task<FloorDTO> GetFloorByIdAsync(int floorId)
        {
            var floor = await _unitOfWork.Floors.GetByIdAsync(floorId);
            if (floor == null)
            {
                throw new NotFoundException(nameof(Floor), floorId);
            }
            return _mapper.Map<FloorDTO>(floor);
        }

        public async Task<FloorDTO> GetFloorWithDesksAsync(int floorId)
        {
            var floor = await _unitOfWork.Floors.GetByIdAsync(floorId);
            if (floor == null)
            {
                throw new NotFoundException(nameof(Floor), floorId);
            }
            return _mapper.Map<FloorDTO>(floor);
        }

        public async Task<IEnumerable<FloorDTO>> GetFloorsWithDesksAsync()
        {
            var floors = await _unitOfWork.Floors.GetFloorsWithDesksAsync();
            return _mapper.Map<IEnumerable<FloorDTO>>(floors);
        }

        public async Task UpdateFloorAsync(int floorId, UpdateFloorRequestDTO updateFloorRequest)
        {
            var floor = await _unitOfWork.Floors.GetByIdAsync(floorId);
            if (floor == null)
            {
                throw new NotFoundException(nameof(Floor), floorId);
            }

            var floorExists = await _unitOfWork.Floors.FloorNumberExistsAsync(updateFloorRequest.FloorNumber);
            if (floorExists && floor.FloorNumber != updateFloorRequest.FloorNumber)
            {
                throw new BadRequestException($"Floor with number {updateFloorRequest.FloorNumber} already exists.");
            }

            _mapper.Map(updateFloorRequest, floor);
            _unitOfWork.Floors.Update(floor);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
