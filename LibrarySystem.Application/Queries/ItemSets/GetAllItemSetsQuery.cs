using AutoMapper;
using LibrarySystem.Application.DTOs.ItemSets;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.ItemSets;

public record GetAllItemSetsQuery() : IRequest<IEnumerable<ItemSetResponse>>;

public class GetAllItemSetsQueryHandler : IRequestHandler<GetAllItemSetsQuery, IEnumerable<ItemSetResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllItemSetsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ItemSetResponse>> Handle(GetAllItemSetsQuery request, CancellationToken cancellationToken)
    {
        var itemSets = await _unitOfWork.ItemSets.GetAllAsync();
        return _mapper.Map<IEnumerable<ItemSetResponse>>(itemSets);
    }
}
