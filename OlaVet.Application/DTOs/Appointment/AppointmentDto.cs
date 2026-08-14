// =============================================
// File: OlaVet.Application/DTOs/Appointment/AppointmentDto.cs
// DTOs for Appointment operations
// =============================================

namespace OlaVet.Application.DTOs.Appointment;

/// <summary>
/// Vet appointment response DTO
/// </summary>
public record VetAppointmentDto
{
    public int VetAppointmentId { get; init; }
    public int PetId { get; init; }
    public string PetName { get; init; } = string.Empty;
    public int VetId { get; init; }
    public string VetName { get; init; } = string.Empty;
    public DateTime AppointmentDate { get; init; }
    public string AppointmentType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal Fee { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedDate { get; init; }
}

/// <summary>
/// Lab appointment response DTO
/// </summary>
public record LabAppointmentDto
{
    public int LabAppointmentId { get; init; }
    public int PetId { get; init; }
    public string PetName { get; init; } = string.Empty;
    public int LabId { get; init; }
    public string LabName { get; init; } = string.Empty;
    public DateTime AppointmentDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string? Notes { get; init; }
    public List<LabTestDto> Tests { get; init; } = new();
}

/// <summary>
/// Lab test info
/// </summary>
public record LabTestDto
{
    public int LabTestId { get; init; }
    public string TestName { get; init; } = string.Empty;
    public decimal TestPrice { get; init; }
    public string? Result { get; init; }
}

/// <summary>
/// Request for creating vet appointment
/// </summary>
public record CreateVetAppointmentDto
{
    public int PetId { get; init; }
    public int VetId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public int VetAppointmentTypeId { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// Request for creating lab appointment
/// </summary>
public record CreateLabAppointmentDto
{
    public int PetId { get; init; }
    public int LabId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public List<int> TestIds { get; init; } = new();
    public string? Notes { get; init; }
}

/// <summary>
/// Request for updating appointment status
/// </summary>
public record UpdateAppointmentStatusDto
{
    public int StatusId { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// Available time slot
/// </summary>
public record TimeSlotDto
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public bool IsAvailable { get; init; }
}

/// <summary>
/// Request for searching appointments
/// </summary>
public record SearchAppointmentsDto
{
    public int? PetId { get; init; }
    public int? VetId { get; init; }
    public int? LabId { get; init; }
    public int? OwnerId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int? StatusId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
