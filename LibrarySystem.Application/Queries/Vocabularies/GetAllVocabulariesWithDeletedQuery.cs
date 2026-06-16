using AutoMapper;
using LibrarySystem.Application.DTOs; // تأكد من مسار الـ VocabularyAdminResponse
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries // يفضل أن تضعها في مسار .Vocabularies
{
    public record GetAllVocabulariesWithDeletedQuery() : IRequest<IEnumerable<VocabularyAdminResponse>>;

    public class GetAllVocabulariesWithDeletedQueryHandler : IRequestHandler<GetAllVocabulariesWithDeletedQuery, IEnumerable<VocabularyAdminResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllVocabulariesWithDeletedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VocabularyAdminResponse>> Handle(GetAllVocabulariesWithDeletedQuery request, CancellationToken cancellationToken)
        {
            // 1. استخدام الدالة التي تجلب المحذوف
            var vocabularies = await _unitOfWork.Vocabularies.GetAllWithDeletedAsync();

            // 2. التحويل إلى DTO الخاص بالإدمن
            var response = _mapper.Map<List<VocabularyAdminResponse>>(vocabularies);

            return response;
        }
    }
}