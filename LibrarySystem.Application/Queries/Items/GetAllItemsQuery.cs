using AutoMapper;
using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Items;

public record GetAllItemsQuery() : IRequest<IEnumerable<ItemResponse>>;

public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, IEnumerable<ItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ItemResponse>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch all items with Owner
        var items = await _unitOfWork.Items.FindAsync(
            x => true,
            i => i.Owner!
        );

        var itemList = items.ToList();
        if (!itemList.Any()) return new List<ItemResponse>();

        // 2. Fetch ALL values for ALL items in one single DB call
        var itemIds = itemList.Select(i => i.Id).ToList();
        var allValues = await _unitOfWork.Values.FindAsync(
            v => itemIds.Contains(v.ResourceId),
            v => v.Property!
        );

        // 3. Group values by ResourceId for fast lookup
        var valuesByItemId = allValues
            .GroupBy(v => v.ResourceId)
            .ToDictionary(g => g.Key, g => g.ToList());
        // 4. Map and inject values for each item
        var result = itemList.Select(item =>
        {
            var response = _mapper.Map<ItemResponse>(item);

            response.MetadataValues = valuesByItemId
                .TryGetValue(item.Id, out var values)
                ? values.Select(v => new ItemValueResponse
                {
                    PropertyId = v.PropertyId,
                    PropertyLabel = v.Property?.Label ?? string.Empty,
                    ValueText = v.ValueText,
                    Language = v.Language
                }).ToList()
                : new List<ItemValueResponse>();

            return response;
        }).ToList();

        return result;
    }
}