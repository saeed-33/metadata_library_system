using AutoMapper;
using LibrarySystem.Application.DTOs.ItemCopies;
using LibrarySystem.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Queries.ItemCopies;

public record GetItemCopyByIdQuery(int Id) : IRequest<ItemCopyResponse?>;

public class GetItemCopyByIdQueryHandler : IRequestHandler<GetItemCopyByIdQuery, ItemCopyResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetItemCopyByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ItemCopyResponse?> Handle(GetItemCopyByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.ItemCopies.GetByIdAsync(request.Id);
        return _mapper.Map<ItemCopyResponse?>(entity);
    }
}
