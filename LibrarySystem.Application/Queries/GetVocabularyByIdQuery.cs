using AutoMapper;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetVocabularyByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<VocabularyResponse?> Handle(GetVocabularyByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Get from repository
            var vocabulary = await _unitOfWork.Vocabularies.GetByIdAsync(request.Id);

            if (vocabulary == null)
            {
                return null; // Or throw a NotFoundException depending on your error handling strategy
            }

            // 2. Map Entity to DTO
            return _mapper.Map<VocabularyResponse>(vocabulary);
        }
    }

}
