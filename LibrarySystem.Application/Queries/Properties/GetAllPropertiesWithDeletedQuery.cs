using AutoMapper;
using LibrarySystem.Application.DTOs.Properties;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Properties;

public record GetAllPropertiesWithDeletedQuery() : IRequest<IEnumerable<PropertyAdminResponse>>;

public class GetAllPropertiesWithDeletedQueryHandler : IRequestHandler<GetAllPropertiesWithDeletedQuery, IEnumerable<PropertyAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPropertiesWithDeletedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PropertyAdminResponse>> Handle(GetAllPropertiesWithDeletedQuery request, CancellationToken cancellationToken)
    {
        // استخدام FindWithDeletedAsync بدلاً من FindAsync
        var properties = await _unitOfWork.Properties.FindWithDeletedAsync(
            p => true,
            p => p.Vocabulary!
        );

        return _mapper.Map<IEnumerable<PropertyAdminResponse>>(properties);
    }
}