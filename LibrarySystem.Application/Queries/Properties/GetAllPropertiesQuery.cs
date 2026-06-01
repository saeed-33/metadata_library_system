using AutoMapper;
using LibrarySystem.Application.DTOs.Properties;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Properties;

public record GetAllPropertiesQuery() : IRequest<IEnumerable<PropertyResponse>>;

public class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, IEnumerable<PropertyResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPropertiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PropertyResponse>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
    {
        // Include Vocabulary so VocabularyPrefix gets populated in the response
        var properties = await _unitOfWork.Properties.FindAsync(
            p => true,
            p => p.Vocabulary!
        );

        return _mapper.Map<IEnumerable<PropertyResponse>>(properties);
    }
}