using AutoMapper;
using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Items;

public record GetAllItemsWithDeletedQuery() : IRequest<IEnumerable<ItemAdminResponse>>;

public class GetAllItemsWithDeletedQueryHandler : IRequestHandler<GetAllItemsWithDeletedQuery, IEnumerable<ItemAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllItemsWithDeletedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ItemAdminResponse>> Handle(GetAllItemsWithDeletedQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch all items (including deleted) with Owner
        var items = await _unitOfWork.Items.FindWithDeletedAsync(
            x => true,
            i => i.Owner!
        );

        var itemList = items.ToList();
        if (!itemList.Any()) return new List<ItemAdminResponse>();

        // 2. Fetch ALL values (including deleted ones) for ALL items
        var itemIds = itemList.Select(i => i.Id).ToList();
        var allValues = await _unitOfWork.Values.FindWithDeletedAsync(
            v => itemIds.Contains(v.ResourceId),
            v => v.Property!
        );

        // 3. Group values by ResourceId
        var valuesByItemId = allValues
            .GroupBy(v => v.ResourceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 4. Map to ItemAdminResponse and inject values
        var result = itemList.Select(item =>
        {
            var response = _mapper.Map<ItemAdminResponse>(item);

            response.MetadataValues = valuesByItemId
                .TryGetValue(item.Id, out var values)
                ? values.Select(v => new ItemAdminValueResponse
                {
                    PropertyId = v.PropertyId,
                    PropertyLabel = v.Property?.Label ?? string.Empty,
                    ValueText = v.ValueText,
                    Language = v.Language,
                    IsDeleted = v.IsDeleted // إظهار حالة الحذف للقيمة أيضاً
                }).ToList()
                : new List<ItemAdminValueResponse>();

            return response;
        }).ToList();

        return result;
    }
}