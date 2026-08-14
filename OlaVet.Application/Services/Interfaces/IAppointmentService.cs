// =============================================
// File: OlaVet.Application/Services/Interfaces/IAppointmentService.cs
// Service interface for Appointment business logic
// =============================================

using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Appointment;
using OlaVet.Domain.Common;

namespace OlaVet.Application.Services.Interfaces;

public interface IAppointmentService
{
    // Vet Appointments
    Task<Result<VetAppointmentDto>> GetVetAppointmentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<VetAppointmentDto>>> GetVetAppointmentsAsync(SearchAppointmentsDto search, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetAppointmentDto>>> GetVetAppointmentsByPetAsync(int petId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetAppointmentDto>>> GetVetAppointmentsByVetAsync(int vetId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetAppointmentDto>>> GetVetAppointmentsByOwnerAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<Result<VetAppointmentDto>> CreateVetAppointmentAsync(CreateVetAppointmentDto dto, CancellationToken cancellationToken = default);
    Task<Result<VetAppointmentDto>> UpdateVetAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto, CancellationToken cancellationToken = default);
    Task<Result> CancelVetAppointmentAsync(int id, CancellationToken cancellationToken = default);
    
    // Lab Appointments
    Task<Result<LabAppointmentDto>> GetLabAppointmentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<LabAppointmentDto>>> GetLabAppointmentsAsync(SearchAppointmentsDto search, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LabAppointmentDto>>> GetLabAppointmentsByPetAsync(int petId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LabAppointmentDto>>> GetLabAppointmentsByLabAsync(int labId, CancellationToken cancellationToken = default);
    Task<Result<LabAppointmentDto>> CreateLabAppointmentAsync(CreateLabAppointmentDto dto, CancellationToken cancellationToken = default);
    Task<Result<LabAppointmentDto>> UpdateLabAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto, CancellationToken cancellationToken = default);
    Task<Result> CancelLabAppointmentAsync(int id, CancellationToken cancellationToken = default);
    
    // Availability
    Task<Result<IEnumerable<TimeSlotDto>>> GetAvailableSlotsAsync(int vetId, DateTime date, CancellationToken cancellationToken = default);
}
