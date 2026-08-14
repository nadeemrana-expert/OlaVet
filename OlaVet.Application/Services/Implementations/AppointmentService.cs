// =============================================
// File: OlaVet.Application/Services/Implementations/AppointmentService.cs
// Service implementation for Appointment business logic
// =============================================

using AutoMapper;
using FluentValidation;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Appointment;
using OlaVet.Application.Exceptions;
using OlaVet.Application.Services.Interfaces;
using OlaVet.Domain.Common;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.Application.Services.Implementations;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateVetAppointmentDto> _createVetAppointmentValidator;
    private readonly IValidator<CreateLabAppointmentDto> _createLabAppointmentValidator;
    
    private const int StatusScheduled = 1;
    private const int StatusConfirmed = 2;
    private const int StatusCompleted = 3;
    private const int StatusCancelled = 4;
    
    public AppointmentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateVetAppointmentDto> createVetAppointmentValidator,
        IValidator<CreateLabAppointmentDto> createLabAppointmentValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createVetAppointmentValidator = createVetAppointmentValidator;
        _createLabAppointmentValidator = createLabAppointmentValidator;
    }
    
    // =============================================
    // VET APPOINTMENTS
    // =============================================
    
    public async Task<Result<VetAppointmentDto>> GetVetAppointmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.VetAppointments.GetWithDetailsAsync(id);
        
        if (appointment == null)
            return Result<VetAppointmentDto>.Failure($"Vet appointment with ID {id} not found");
        
        return Result<VetAppointmentDto>.Success(_mapper.Map<VetAppointmentDto>(appointment));
    }
    
    public async Task<Result<PagedResult<VetAppointmentDto>>> GetVetAppointmentsAsync(SearchAppointmentsDto search, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.VetAppointments.GetPagedAsync(search.Page, search.PageSize, null, null, true, cancellationToken);
        
        var dtos = new PagedResult<VetAppointmentDto>(
            _mapper.Map<IEnumerable<VetAppointmentDto>>(result.Items),
            result.TotalCount,
            result.Page,
            result.PageSize
        );
        
        return Result<PagedResult<VetAppointmentDto>>.Success(dtos);
    }
    
    public async Task<Result<IEnumerable<VetAppointmentDto>>> GetVetAppointmentsByPetAsync(int petId, CancellationToken cancellationToken = default)
    {
        var appointments = await _unitOfWork.VetAppointments.GetPetHistoryAsync(petId);
        return Result<IEnumerable<VetAppointmentDto>>.Success(_mapper.Map<IEnumerable<VetAppointmentDto>>(appointments));
    }
    
    public async Task<Result<IEnumerable<VetAppointmentDto>>> GetVetAppointmentsByVetAsync(int vetId, CancellationToken cancellationToken = default)
    {
        // Use GetByVetAndDateAsync with a wide date range or get upcoming
        var appointments = await _unitOfWork.VetAppointments.GetUpcomingAsync(365);
        var vetAppointments = appointments.Where(a => a.VetId == vetId);
        return Result<IEnumerable<VetAppointmentDto>>.Success(_mapper.Map<IEnumerable<VetAppointmentDto>>(vetAppointments));
    }
    
    public async Task<Result<IEnumerable<VetAppointmentDto>>> GetVetAppointmentsByOwnerAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        var appointments = await _unitOfWork.VetAppointments.GetByOwnerIdAsync(ownerId);
        return Result<IEnumerable<VetAppointmentDto>>.Success(_mapper.Map<IEnumerable<VetAppointmentDto>>(appointments));
    }
    
    public async Task<Result<VetAppointmentDto>> CreateVetAppointmentAsync(CreateVetAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createVetAppointmentValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<VetAppointmentDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Verify pet exists
        var pet = await _unitOfWork.Pets.GetWithOwnerAsync(dto.PetId);
        if (pet == null)
            return Result<VetAppointmentDto>.Failure($"Pet with ID {dto.PetId} not found");
        
        // Verify vet exists
        var vet = await _unitOfWork.Vets.GetByIdAsync(dto.VetId, cancellationToken);
        if (vet == null)
            return Result<VetAppointmentDto>.Failure($"Vet with ID {dto.VetId} not found");
        
        // Check owner has enough funds
        var owner = pet.PetOwner;
        if (owner.Wallet < vet.Fee)
            throw new InsufficientFundsException(vet.Fee, owner.Wallet);
        
        // Check time slot availability
        var isAvailable = await _unitOfWork.VetAppointments.IsTimeSlotAvailableAsync(dto.VetId, dto.AppointmentDate);
        if (!isAvailable)
            return Result<VetAppointmentDto>.Failure("The selected time slot is not available");
        
        // Create appointment
        var appointment = new VetAppointment
        {
            PetId = dto.PetId,
            PetOwnerId = owner.PetOwnerId,
            VetId = dto.VetId,
            AppointmentDateTime = dto.AppointmentDate,
            VetAppointmentTypeId = dto.VetAppointmentTypeId,
            StatusTypeId = StatusScheduled,
            Reason = dto.Notes,
            CreatedDate = DateTime.UtcNow
        };
        
        await _unitOfWork.VetAppointments.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Load related data for response
        var created = await _unitOfWork.VetAppointments.GetWithDetailsAsync(appointment.VetAppointmentId);
        
        return Result<VetAppointmentDto>.Success(_mapper.Map<VetAppointmentDto>(created));
    }
    
    public async Task<Result<VetAppointmentDto>> UpdateVetAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.VetAppointments.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            return Result<VetAppointmentDto>.Failure($"Vet appointment with ID {id} not found");
        
        appointment.StatusTypeId = dto.StatusId;
        if (dto.Notes != null)
            appointment.Notes = dto.Notes;
        
        if (dto.StatusId == StatusCompleted)
            appointment.CompletedDate = DateTime.UtcNow;
        
        _unitOfWork.VetAppointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var updated = await _unitOfWork.VetAppointments.GetWithDetailsAsync(id);
        
        return Result<VetAppointmentDto>.Success(_mapper.Map<VetAppointmentDto>(updated));
    }
    
    public async Task<Result> CancelVetAppointmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.VetAppointments.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            return Result.Failure($"Vet appointment with ID {id} not found");
        
        if (appointment.StatusTypeId == StatusCompleted)
            return Result.Failure("Cannot cancel a completed appointment");
        
        if (appointment.StatusTypeId == StatusCancelled)
            return Result.Failure("Appointment is already cancelled");
        
        appointment.StatusTypeId = StatusCancelled;
        
        _unitOfWork.VetAppointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    // =============================================
    // LAB APPOINTMENTS
    // =============================================
    
    public async Task<Result<LabAppointmentDto>> GetLabAppointmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.LabAppointments.GetWithDetailsAsync(id);
        
        if (appointment == null)
            return Result<LabAppointmentDto>.Failure($"Lab appointment with ID {id} not found");
        
        return Result<LabAppointmentDto>.Success(_mapper.Map<LabAppointmentDto>(appointment));
    }
    
    public async Task<Result<PagedResult<LabAppointmentDto>>> GetLabAppointmentsAsync(SearchAppointmentsDto search, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.LabAppointments.GetPagedAsync(search.Page, search.PageSize, null, null, true, cancellationToken);
        
        var dtos = new PagedResult<LabAppointmentDto>(
            _mapper.Map<IEnumerable<LabAppointmentDto>>(result.Items),
            result.TotalCount,
            result.Page,
            result.PageSize
        );
        
        return Result<PagedResult<LabAppointmentDto>>.Success(dtos);
    }
    
    public async Task<Result<IEnumerable<LabAppointmentDto>>> GetLabAppointmentsByPetAsync(int petId, CancellationToken cancellationToken = default)
    {
        var appointments = await _unitOfWork.LabAppointments.GetByPetIdAsync(petId);
        return Result<IEnumerable<LabAppointmentDto>>.Success(_mapper.Map<IEnumerable<LabAppointmentDto>>(appointments));
    }
    
    public async Task<Result<IEnumerable<LabAppointmentDto>>> GetLabAppointmentsByLabAsync(int labId, CancellationToken cancellationToken = default)
    {
        // ILabAppointmentRepository doesn't have GetByLabIdAsync, use filter
        var appointments = await _unitOfWork.LabAppointments.GetUpcomingAsync(365);
        var labAppointments = appointments.Where(a => a.LabId == labId);
        return Result<IEnumerable<LabAppointmentDto>>.Success(_mapper.Map<IEnumerable<LabAppointmentDto>>(labAppointments));
    }
    
    public async Task<Result<LabAppointmentDto>> CreateLabAppointmentAsync(CreateLabAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createLabAppointmentValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<LabAppointmentDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Verify pet exists
        var pet = await _unitOfWork.Pets.GetWithOwnerAsync(dto.PetId);
        if (pet == null)
            return Result<LabAppointmentDto>.Failure($"Pet with ID {dto.PetId} not found");
        
        // Verify lab exists
        var lab = await _unitOfWork.Labs.GetByIdAsync(dto.LabId, cancellationToken);
        if (lab == null)
            return Result<LabAppointmentDto>.Failure($"Lab with ID {dto.LabId} not found");
        
        // Create appointment
        var appointment = new LabAppointment
        {
            PetId = dto.PetId,
            PetOwnerId = pet.PetOwnerId,
            LabId = dto.LabId,
            AppointmentDateTime = dto.AppointmentDate,
            StatusTypeId = StatusScheduled,
            Notes = dto.Notes,
            CreatedDate = DateTime.UtcNow
        };
        
        await _unitOfWork.LabAppointments.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<LabAppointmentDto>.Success(_mapper.Map<LabAppointmentDto>(appointment));
    }
    
    public async Task<Result<LabAppointmentDto>> UpdateLabAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.LabAppointments.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            return Result<LabAppointmentDto>.Failure($"Lab appointment with ID {id} not found");
        
        appointment.StatusTypeId = dto.StatusId;
        if (dto.Notes != null)
            appointment.Notes = dto.Notes;
        
        if (dto.StatusId == StatusCompleted)
            appointment.CompletedDate = DateTime.UtcNow;
        
        _unitOfWork.LabAppointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<LabAppointmentDto>.Success(_mapper.Map<LabAppointmentDto>(appointment));
    }
    
    public async Task<Result> CancelLabAppointmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.LabAppointments.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            return Result.Failure($"Lab appointment with ID {id} not found");
        
        if (appointment.StatusTypeId == StatusCompleted)
            return Result.Failure("Cannot cancel a completed appointment");
        
        if (appointment.StatusTypeId == StatusCancelled)
            return Result.Failure("Appointment is already cancelled");
        
        appointment.StatusTypeId = StatusCancelled;
        
        _unitOfWork.LabAppointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    // =============================================
    // AVAILABILITY
    // =============================================
    
    public async Task<Result<IEnumerable<TimeSlotDto>>> GetAvailableSlotsAsync(int vetId, DateTime date, CancellationToken cancellationToken = default)
    {
        var vet = await _unitOfWork.Vets.GetByIdAsync(vetId, cancellationToken);
        if (vet == null)
            return Result<IEnumerable<TimeSlotDto>>.Failure($"Vet with ID {vetId} not found");
        
        // Get available time slots from repository
        var availableSlots = await _unitOfWork.VetAppointments.GetAvailableTimeSlotsAsync(vetId, date);
        
        var slots = availableSlots.Select(slotStart => new TimeSlotDto
        {
            StartTime = slotStart,
            EndTime = slotStart.AddMinutes(30),
            IsAvailable = true
        }).ToList();
        
        return Result<IEnumerable<TimeSlotDto>>.Success(slots);
    }
}
