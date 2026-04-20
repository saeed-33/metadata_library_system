using AutoMapper;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Mappings
{ 
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Vocabulary, VocabularyResponse>().ReverseMap();

        }
    }
}
