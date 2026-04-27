using AutoMapper;
using LibrarySystem.Application.DTOs.ResourceTemplates;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.ResourceTemplates;

public record GetAllResourceTemplatesQuery() : IRequest<IEnumerable<ResourceTemplateResponse>>;

public class GetAllResourceTemplatesQueryHandler : IRequestHandler<GetAllResourceTemplatesQuery, IEnumerable<ResourceTemplateResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllResourceTemplatesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResourceTemplateResponse>> Handle(GetAllResourceTemplatesQuery request, CancellationToken cancellationToken)
    {
        // جلب كل القوالب المتاحة (غير المحذوفة)
        var templates = await _unitOfWork.ResourceTemplates.GetAllAsync();

        return _mapper.Map<IEnumerable<ResourceTemplateResponse>>(templates);
    }
}