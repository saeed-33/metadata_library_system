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
        // نجلب الـ Items مع المالك والقيم الوصفية لكل كتاب
        var items = await _unitOfWork.Items.FindAsync(
            x => true,
            i => i.Owner!,
            i => i.Values
        );
        return _mapper.Map<IEnumerable<ItemResponse>>(items);
    }
}