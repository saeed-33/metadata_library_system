using AutoMapper;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Queries
{
    public record GetAllVocabulariesQuery() : IRequest<IEnumerable<VocabularyResponse>>;

    // 2. The Handler
    public class GetAllVocabulariesQueryHandler : IRequestHandler<GetAllVocabulariesQuery, IEnumerable<VocabularyResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Vocabulary> _repository;

        public GetAllVocabulariesQueryHandler(IGenericRepository<Vocabulary> repository,IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IEnumerable<VocabularyResponse>> Handle(GetAllVocabulariesQuery request, CancellationToken cancellationToken)
        {
            // 1. Get all from repository
            var vocabularies = await _repository.GetAllAsync();

            // 2. Map Entities to DTOs
          

            var response = _mapper.Map<List<VocabularyResponse>>(vocabularies);

            return response;
        }
    }
}
