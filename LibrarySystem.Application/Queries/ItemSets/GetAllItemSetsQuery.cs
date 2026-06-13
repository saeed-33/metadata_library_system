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

    public async Task<IEnumerable<ItemSetResponse>> Handle(
        GetAllItemSetsQuery request, CancellationToken cancellationToken)
    {
        var itemSets = await _unitOfWork.ItemSets.GetAllAsync();

        return itemSets.Select(itemSet =>
        {
            var response = _mapper.Map<ItemSetResponse>(itemSet);

            // Inject items manually since mapping ignores them
            response.Items = itemSet.Items.Select(item => new ItemSetItemResponse
            {
                Id = item.Id,
                Type = item.Type,
                TemplateId = item.TemplateId,
                OwnerId = item.OwnerId
            }).ToList();

            return response;
        }).ToList();
    }
}