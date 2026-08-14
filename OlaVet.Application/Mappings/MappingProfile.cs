// =============================================
// File: OlaVet.Application/Mappings/MappingProfile.cs
// AutoMapper profile for all entity-to-DTO mappings
// =============================================

using AutoMapper;
using OlaVet.Application.DTOs.Appointment;
using OlaVet.Application.DTOs.Order;
using OlaVet.Application.DTOs.Pet;
using OlaVet.Application.DTOs.PetOwner;
using OlaVet.Application.DTOs.Review;
using OlaVet.Application.DTOs.Vet;
using OlaVet.Domain.Entities;

namespace OlaVet.Application.Mappings;

/// <summary>
/// AutoMapper profile containing all entity-to-DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // =============================================
        // PET OWNER MAPPINGS
        // =============================================
        
        CreateMap<PetOwner, PetOwnerDto>();
        
        CreateMap<PetOwner, PetOwnerDetailsDto>()
            .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.Pets));
        
        CreateMap<Pet, PetSummaryDto>();
        
        CreateMap<CreatePetOwnerDto, PetOwner>()
            .ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.InitialWalletBalance))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        CreateMap<UpdatePetOwnerDto, PetOwner>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // =============================================
        // VET MAPPINGS
        // =============================================
        
        CreateMap<Vet, VetDto>();
        
        CreateMap<Vet, VetWithRatingDto>()
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                src.VetReviews.Any() ? src.VetReviews.Average(r => r.Rating) : 0))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.VetReviews.Count));
        
        CreateMap<Vet, VetDetailsDto>()
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                src.VetReviews.Any() ? src.VetReviews.Average(r => r.Rating) : 0))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.VetReviews.Count))
            .ForMember(dest => dest.Qualifications, opt => opt.MapFrom(src => src.EducationQualifications))
            .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availabilities));
        
        CreateMap<EducationQualification, QualificationDto>();
        CreateMap<Service, ServiceDto>();
        CreateMap<VetAvailability, AvailabilityDto>();
        
        CreateMap<CreateVetDto, Vet>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        CreateMap<UpdateVetDto, Vet>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // =============================================
        // PET MAPPINGS
        // =============================================
        
        CreateMap<Pet, PetDto>();
        
        CreateMap<Pet, PetWithOwnerDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.PetOwner.OwnerName))
            .ForMember(dest => dest.OwnerContactNumber, opt => opt.MapFrom(src => src.PetOwner.ContactNumber));
        
        CreateMap<Pet, PetDetailsDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.PetOwner.OwnerName))
            .ForMember(dest => dest.OwnerContactNumber, opt => opt.MapFrom(src => src.PetOwner.ContactNumber))
            .ForMember(dest => dest.MedicalHistory, opt => opt.MapFrom(src => src.MedicalRecords))
            .ForMember(dest => dest.TotalAppointments, opt => opt.MapFrom(src => src.VetAppointments.Count));
        
        CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(dest => dest.RecordType, opt => opt.MapFrom(src => src.RecordType.TypeName))
            .ForMember(dest => dest.VetName, opt => opt.MapFrom(src => src.Vet != null ? src.Vet.VetName : null));
        
        CreateMap<CreatePetDto, Pet>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        CreateMap<UpdatePetDto, Pet>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // =============================================
        // APPOINTMENT MAPPINGS
        // =============================================
        
        CreateMap<VetAppointment, VetAppointmentDto>()
            .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet.Name))
            .ForMember(dest => dest.VetName, opt => opt.MapFrom(src => src.Vet.VetName))
            .ForMember(dest => dest.AppointmentType, opt => opt.MapFrom(src => src.VetAppointmentType.TypeName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusType.StatusName));
        
        CreateMap<VetAppointment, AppointmentSummaryDto>()
            .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.VetAppointmentId))
            .ForMember(dest => dest.AppointmentType, opt => opt.MapFrom(src => src.VetAppointmentType.TypeName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusType.StatusName))
            .ForMember(dest => dest.VetName, opt => opt.MapFrom(src => src.Vet.VetName));
        
        CreateMap<LabAppointment, LabAppointmentDto>()
            .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet.Name))
            .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.Lab.LabName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusType.StatusName))
            .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => 
                src.LabAppointmentTests.Select(lat => lat.LabTest)));
        
        CreateMap<LabTest, LabTestDto>();
        
        CreateMap<CreateVetAppointmentDto, VetAppointment>()
            .ForMember(dest => dest.StatusTypeId, opt => opt.MapFrom(_ => 1)) // Scheduled
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        CreateMap<CreateLabAppointmentDto, LabAppointment>()
            .ForMember(dest => dest.StatusTypeId, opt => opt.MapFrom(_ => 1)) // Scheduled
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        // =============================================
        // ORDER MAPPINGS
        // =============================================
        
        CreateMap<MedicineOrder, MedicineOrderDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.PetOwner.OwnerName))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.StoreName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusType.StatusName))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.MedicineOrderDetails));
        
        CreateMap<MedicineOrderDetail, OrderItemDto>()
            .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.MedicineName));
        
        CreateMap<Medicine, MedicineDto>()
            .ForMember(dest => dest.MedicineType, opt => opt.MapFrom(src => src.MedicineType.TypeName))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsActive));
        
        CreateMap<Store, StoreDto>()
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                src.StoreReviews.Any() ? src.StoreReviews.Average(r => r.Rating) : (double?)null))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.StoreReviews.Count));
        
        // =============================================
        // REVIEW MAPPINGS
        // =============================================
        
        CreateMap<VetReview, VetReviewDto>()
            .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.VetReviewId))
            .ForMember(dest => dest.VetName, opt => opt.MapFrom(src => src.Vet.VetName))
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.PetOwner.OwnerName))
            .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.ReviewDateTime))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comments));
        
        CreateMap<LabReview, LabReviewDto>()
            .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.LabReviewId))
            .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.Lab.LabName))
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.PetOwner.OwnerName))
            .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.ReviewDateTime))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comments));
        
        CreateMap<StoreReview, StoreReviewDto>()
            .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.StoreReviewId))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.StoreName))
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.PetOwner.OwnerName))
            .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.ReviewDateTime))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comments));
        
        CreateMap<CreateVetReviewDto, VetReview>()
            .ForMember(dest => dest.ReviewDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comment));
        
        CreateMap<CreateLabReviewDto, LabReview>()
            .ForMember(dest => dest.ReviewDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comment));
        
        CreateMap<CreateStoreReviewDto, StoreReview>()
            .ForMember(dest => dest.ReviewDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comment));
    }
}
