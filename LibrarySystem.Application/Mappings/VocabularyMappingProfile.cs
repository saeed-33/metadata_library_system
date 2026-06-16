using AutoMapper;
using LibrarySystem.Application.Commands;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Mappings
{ 
    public class VocabularyMappingProfile : Profile
    {
        public VocabularyMappingProfile()
        {
            CreateMap<Vocabulary, VocabularyResponse>();
            CreateMap<Vocabulary, VocabularyAdminResponse>();



            CreateMap<CreateVocabularyCommand, Vocabulary>();

            CreateMap<UpdateVocabularyCommand, Vocabulary>();
        }
    }
}
