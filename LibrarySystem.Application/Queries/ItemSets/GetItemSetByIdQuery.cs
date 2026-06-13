using AutoMapper;
using LibrarySystem.Application.DTOs.ItemSets;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.ItemSets;

public record GetItemSetByIdQuery(int Id) : IRequest<ItemSetResponse?>;

public class GetItemSetByIdQueryHandler : IRequestHandler<GetItemSetByIdQuery, ItemSetResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetItemSetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ItemSetResponse?> Handle(
        GetItemSetByIdQuery request, CancellationToken cancellationToken)
    {
        var itemSet = await _unitOfWork.ItemSets.GetByIdAsync(request.Id);
        if (itemSet == null) return null;

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
    }
}