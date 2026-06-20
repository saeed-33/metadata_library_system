using AutoMapper;
using LibrarySystem.Application.Commands.Patrons; // تأكد من مسار الـ Commands
using LibrarySystem.Application.DTOs.Patrons;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class PatronMappingProfile : Profile
{
    public PatronMappingProfile()
    {
        // من Entity إلى DTO
        CreateMap<Patron, PatronResponse>();
        CreateMap<Patron, PatronAdminResponse>();

        // من Commands إلى Entity
        CreateMap<CreatePatronCommand, Patron>();
        CreateMap<UpdatePatronCommand, Patron>();
    }
}