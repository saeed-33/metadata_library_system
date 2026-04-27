using AutoMapper;
using LibrarySystem.Application.DTOs.Properties;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Properties;

public record GetPropertiesByVocabularyQuery(int VocabularyId) : IRequest<IEnumerable<PropertyResponse>>;

public class GetPropertiesByVocabularyQueryHandler : IRequestHandler<GetPropertiesByVocabularyQuery, IEnumerable<PropertyResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPropertiesByVocabularyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PropertyResponse>> Handle(GetPropertiesByVocabularyQuery request, CancellationToken cancellationToken)
    {
        // استخدام Include لجلب بيانات القاموس لكي نملأ الـ Prefix في الـ DTO
        var properties = await _unitOfWork.Properties.FindAsync(
            p => p.VocabularyId == request.VocabularyId,
            p => p.Vocabulary!
        );

        return _mapper.Map<IEnumerable<PropertyResponse>>(properties);
    }
}