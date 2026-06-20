using AutoMapper;
using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Items;

public record GetItemWithCopiesQuery(int Id) : IRequest<ItemResponse?>;

public class GetItemWithCopiesHandler : IRequestHandler<GetItemWithCopiesQuery, ItemResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetItemWithCopiesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ItemResponse?> Handle(GetItemWithCopiesQuery request, CancellationToken ct)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(request.Id);
        if (item == null) return null;

        var allCopies = await _unitOfWork.ItemCopies.GetAllAsync();
        var itemCopies = allCopies.Where(c => c.ItemId == item.Id).ToList();

        var response = _mapper.Map<ItemResponse>(item);
        response.TotalCopies = itemCopies.Count;
        response.AvailableCopies = itemCopies.Count(c => c.Status == 0);

        return response;
    }
}