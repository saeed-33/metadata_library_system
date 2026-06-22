using AutoMapper;
using LibrarySystem.Application.DTOs.Properties;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Properties;

public record GetSearchableFieldsQuery() : IRequest<List<PropertyResponse>>;

public class GetSearchableFieldsHandler : IRequestHandler<GetSearchableFieldsQuery, List<PropertyResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSearchableFieldsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PropertyResponse>> Handle(GetSearchableFieldsQuery request, CancellationToken ct)
    {
        var allProps = await _unitOfWork.Properties.GetAllAsync();
        var searchableProps = allProps.Where(p => p.IsSearchable).ToList();

        return _mapper.Map<List<PropertyResponse>>(searchableProps);
    }
}
