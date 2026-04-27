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
        var items = await _unitOfWork.Items.FindAsync(
            i => i.Id == request.Id,
            i => i.Owner!,
            i => i.Values
        );

        var item = items.FirstOrDefault();
        return item == null ? null : _mapper.Map<ItemResponse>(item);
    }
}