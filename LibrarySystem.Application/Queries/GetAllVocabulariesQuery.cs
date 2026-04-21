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
        private readonly IUnitOfWork _unitOfWork;

        public GetAllVocabulariesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VocabularyResponse>> Handle(GetAllVocabulariesQuery request, CancellationToken cancellationToken)
        {
            // 1. Get all from repository
            var vocabularies = await _unitOfWork.Vocabularies.GetAllAsync();

            // 2. Map Entities to DTOs
            var response = vocabularies.Select(v => new VocabularyResponse(
                v.Id,
                v.Prefix,
                v.NamespaceUri,
                v.Label
            ));

            return response;
        }
    }
}
