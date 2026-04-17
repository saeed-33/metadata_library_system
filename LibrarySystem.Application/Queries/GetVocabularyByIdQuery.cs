using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Queries
{
    public record GetVocabularyByIdQuery(int Id) : IRequest<VocabularyResponse?>;

    // 2. The Handler
    public class GetVocabularyByIdQueryHandler : IRequestHandler<GetVocabularyByIdQuery, VocabularyResponse?>
    {
        private readonly IGenericRepository<Vocabulary> _repository;

        public GetVocabularyByIdQueryHandler(IGenericRepository<Vocabulary> repository)
        {
            _repository = repository;
        }

        public async Task<VocabularyResponse?> Handle(GetVocabularyByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Get from repository
            var vocabulary = await _repository.GetByIdAsync(request.Id);

            if (vocabulary == null)
            {
                return null; // Or throw a NotFoundException depending on your error handling strategy
            }

            // 2. Map Entity to DTO
            return new VocabularyResponse(
                vocabulary.Id,
                vocabulary.Prefix,
                vocabulary.NamespaceUri,
                vocabulary.Label
            );
        }
    }

}
