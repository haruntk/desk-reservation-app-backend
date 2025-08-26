using AutoMapper;
using DeskReservationApp.Application.DTOs.Reservation;
using DeskReservationApp.Application.Exceptions;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Domain.Interfaces;
using DeskReservationApp.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DeskReservationApp.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ReservationStatusOptions _statusOptions;
        private readonly IEmailService _emailService;

        public ReservationService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<ReservationStatusOptions> statusOptions, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _statusOptions = statusOptions.Value;
            _emailService = emailService;
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
            reservation.CreatedAt = DateTime.UtcNow;
            
            // Validate reservation times
            var now = DateTime.UtcNow;
            
            if (createReservationRequest.EndTime <= now && !_statusOptions.AllowPastReservations)
            {
                throw new BadRequestException("Cannot create a reservation with a past end time.");
            }

            if (createReservationRequest.StartTime < now && !_statusOptions.AllowPastReservations)
            {
                throw new BadRequestException("Cannot create a reservation with a past start time.");
            }

            var maxFutureDate = now.AddDays(_statusOptions.MaxAdvanceReservationDays);
            if (createReservationRequest.StartTime > maxFutureDate)
            {
                throw new BadRequestException($"Cannot create a reservation more than {_statusOptions.MaxAdvanceReservationDays} days in advance.");
            }
            TimeSpan timeSpan = createReservationRequest.EndTime - createReservationRequest.StartTime;
            if ((int)timeSpan.TotalMinutes < 10)
            {
                throw new BadRequestException("Cannot create a reservation for less than 10 minutes.");
            }

            // Set status based on start time
            if (createReservationRequest.StartTime <= now && createReservationRequest.EndTime > now)
            {
                reservation.Status = "Active"; // Currently ongoing
            }
            else if (createReservationRequest.StartTime > now)
            {
                reservation.Status = "Scheduled"; // Future reservation
            }
            else if (_statusOptions.AllowPastReservations)
            {
                reservation.Status = "Completed"; // Past reservation (admin created)
            }

            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            // Send confirmation email
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var deskWithFloor = await _unitOfWork.Desks.GetByIdAsync(reservation.DeskId);
                    var subject = "Reservation Confirmation";
                    var body = $"Dear {user.UserName},<br/><br/>" +
                               $"Your desk reservation has been successfully created.<br/><br/>" +
                               $"<b>Details:</b><br/>" +
                               $"Floor: {deskWithFloor?.Floor?.FloorNumber}<br/>" +
                               $"Desk: {deskWithFloor?.DeskName}<br/>" +
                               $"Start Time: {reservation.StartTime:g} (UTC)<br/>" +
                               $"End Time: {reservation.EndTime:g} (UTC)<br/><br/>" +
                               $"Thank you for using our Desk Reservation App!";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                // Here we should log the exception but not let it break the main operation.
                // The reservation was successful, but the email failed.
                // A proper logger should be injected and used here.
                System.Diagnostics.Debug.WriteLine($"Error sending email: {ex.Message}");
            }

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
