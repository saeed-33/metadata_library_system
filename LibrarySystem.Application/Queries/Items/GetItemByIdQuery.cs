using AutoMapper;
using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Items;

public record GetItemByIdQuery(int Id) : IRequest<ItemResponse?>;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ItemResponse?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch item with Owner only — Values cannot be included via navigation
        var items = await _unitOfWork.Items.FindAsync(
            i => i.Id == request.Id,
            i => i.Owner!
        );

        var item = items.FirstOrDefault();
        if (item == null) return null;

        // 2. Fetch values separately for this item
        var values = await _unitOfWork.Values.FindAsync(
            v => v.ResourceId == item.Id,
            v => v.Property!   // include Property to get the label
        );

        // 3. Map item to response
        var response = _mapper.Map<ItemResponse>(item);

        // 4. Inject values manually
        response.MetadataValues = values.Select(v => new ItemValueResponse
        {
            PropertyId = v.PropertyId,
            PropertyLabel = v.Property?.Label ?? string.Empty,
            ValueText = v.ValueText,
            Language = v.Language
        }).ToList();

        return response;
    }
}