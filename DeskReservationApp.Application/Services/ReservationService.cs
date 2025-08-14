using AutoMapper;
using DeskReservationApp.Application.DTOs.Reservation;
using DeskReservationApp.Application.Exceptions;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReservationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CancelReservationAsync(int reservationId, string userId)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (reservation == null)
            {
                throw new NotFoundException(nameof(Reservation), reservationId);
            }

            if (reservation.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this reservation.");
            }

            reservation.Status = "Cancelled";
            _unitOfWork.Reservations.Update(reservation);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> CreateReservationAsync(string userId, CreateReservationRequestDTO createReservationRequest)
        {
            var desk = await _unitOfWork.Desks.GetByIdAsync(createReservationRequest.DeskId);
            if (desk == null)
            {
                throw new BadRequestException($"Desk with id {createReservationRequest.DeskId} does not exist.");
            }

            var overlappingReservation = await _unitOfWork.Reservations.HasOverlappingReservationAsync(
                createReservationRequest.DeskId, createReservationRequest.StartTime, createReservationRequest.EndTime);
            if (overlappingReservation)
            {
                throw new BadRequestException("The selected time slot is already booked.");
            }

            var reservation = _mapper.Map<Reservation>(createReservationRequest);
            reservation.UserId = userId;
            reservation.Status = "Active";
            reservation.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            return reservation.ReservationId;
        }

        public async Task DeleteReservationAsync(int reservationId)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (reservation == null)
            {
                throw new NotFoundException(nameof(Reservation), reservationId);
            }

            _unitOfWork.Reservations.Delete(reservation);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReservationResponseDTO>> GetActiveReservationsAsync()
        {
            var reservations = await _unitOfWork.Reservations.GetActiveReservationsAsync();
            return _mapper.Map<IEnumerable<ReservationResponseDTO>>(reservations);
        }

        public async Task<IEnumerable<ReservationResponseDTO>> GetAllReservationsAsync()
        {
            var reservations = await _unitOfWork.Reservations.GetAllAsync();
            return _mapper.Map<IEnumerable<ReservationResponseDTO>>(reservations);
        }

        public async Task<IEnumerable<ReservationResponseDTO>> GetPastReservationsAsync(string userId)
        {
            var reservations = await _unitOfWork.Reservations.GetPastReservationsAsync(userId);
            return _mapper.Map<IEnumerable<ReservationResponseDTO>>(reservations);
        }

        public async Task<ReservationDTO> GetReservationByIdAsync(int reservationId)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (reservation == null)
            {
                throw new NotFoundException(nameof(Reservation), reservationId);
            }
            return _mapper.Map<ReservationDTO>(reservation);
        }

        public async Task<IEnumerable<ReservationResponseDTO>> GetReservationsByDeskIdAsync(int deskId)
        {
            var reservations = await _unitOfWork.Reservations.GetReservationsByDeskIdAsync(deskId);
            return _mapper.Map<IEnumerable<ReservationResponseDTO>>(reservations);
        }

        public async Task<IEnumerable<ReservationResponseDTO>> GetUpcomingReservationsAsync(string userId)
        {
            var reservations = await _unitOfWork.Reservations.GetUpcomingReservationsAsync(userId);
            return _mapper.Map<IEnumerable<ReservationResponseDTO>>(reservations);
        }

        public async Task<UserReservationsResponseDTO> GetUserReservationsAsync(string userId)
        {
            var allReservations = await _unitOfWork.Reservations.GetReservationsByUserIdAsync(userId);
            var pastReservations = await _unitOfWork.Reservations.GetPastReservationsAsync(userId);
            var upcomingReservations = await _unitOfWork.Reservations.GetUpcomingReservationsAsync(userId);

            // Active reservations: currently ongoing (started but not ended and status is Active)
            var now = DateTime.UtcNow;
            var activeReservations = allReservations.Where(r => 
                r.Status == "Active" && 
                r.StartTime <= now && 
                r.EndTime > now).ToList();

            return new UserReservationsResponseDTO
            {
                UserId = userId,
                ActiveReservations = _mapper.Map<List<ReservationResponseDTO>>(activeReservations),
                PastReservations = _mapper.Map<List<ReservationResponseDTO>>(pastReservations),
                UpcomingReservations = _mapper.Map<List<ReservationResponseDTO>>(upcomingReservations),
                TotalReservations = allReservations.Count()
            };
        }

        public async Task UpdateReservationAsync(int reservationId, string userId, UpdateReservationRequestDTO updateReservationRequest)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (reservation == null)
            {
                throw new NotFoundException(nameof(Reservation), reservationId);
            }
            
            if (reservation.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this reservation.");
            }

            // Check if the new desk exists
            var desk = await _unitOfWork.Desks.GetByIdAsync(updateReservationRequest.DeskId);
            if (desk == null)
            {
                throw new BadRequestException($"Desk with id {updateReservationRequest.DeskId} does not exist.");
            }

            // Check for overlapping reservations
            var overlappingReservation = await _unitOfWork.Reservations.HasOverlappingReservationAsync(
                updateReservationRequest.DeskId, updateReservationRequest.StartTime, updateReservationRequest.EndTime, reservationId);
            if (overlappingReservation)
            {
                throw new BadRequestException("The selected time slot is already booked.");
            }
            
            _mapper.Map(updateReservationRequest, reservation);
            _unitOfWork.Reservations.Update(reservation);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateReservationStatusAsync(int reservationId, string userId, UpdateReservationStatusRequestDTO updateStatusRequest)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (reservation == null)
            {
                throw new NotFoundException(nameof(Reservation), reservationId);
            }

            if (reservation.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this reservation's status.");
            }
            
            reservation.Status = updateStatusRequest.Status;
            _unitOfWork.Reservations.Update(reservation);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
