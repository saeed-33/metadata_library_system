using AutoMapper;
using LibrarySystem.Application.Commands.Circulation; // مسار الـ CheckoutCommand
using LibrarySystem.Application.DTOs.Circulation;
using LibrarySystem.Application.DTOs.BorrowRecords;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class BorrowRecordMappingProfile : Profile
{
    public BorrowRecordMappingProfile()
    {
        CreateMap<BorrowRecord, BorrowRecordResponse>();

        CreateMap<BorrowRecord, BorrowRecordAdminResponse>()
            .ForMember(dest => dest.CopyBarcode, opt => opt.MapFrom(src => src.Copy.Barcode))
            .ForMember(dest => dest.PatronName, opt => opt.MapFrom(src => src.Patron.FullName));

        // من Command الإعارة إلى Entity
        CreateMap<CheckoutCommand, BorrowRecord>();

        // Entity → Active loan DTO
        CreateMap<BorrowRecord, ActiveLoanDto>()
            .ForMember(dest => dest.RecordId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CopyBarcode, opt => opt.MapFrom(src => src.Copy.Barcode))
            .ForMember(dest => dest.PatronName, opt => opt.MapFrom(src => src.Patron.FullName));
    }
}
