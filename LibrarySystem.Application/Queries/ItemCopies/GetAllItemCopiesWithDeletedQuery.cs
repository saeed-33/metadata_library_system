using AutoMapper;
using LibrarySystem.Application.DTOs.ItemCopies;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.ItemCopies;

public record GetAllItemCopiesWithDeletedQuery() : IRequest<List<ItemCopyAdminResponse>>;

public class GetAllItemCopiesWithDeletedHandler : IRequestHandler<GetAllItemCopiesWithDeletedQuery, List<ItemCopyAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllItemCopiesWithDeletedHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ItemCopyAdminResponse>> Handle(GetAllItemCopiesWithDeletedQuery request, CancellationToken ct)
    {
        var copies = await _unitOfWork.ItemCopies.GetAllWithDeletedAsync();
        return _mapper.Map<List<ItemCopyAdminResponse>>(copies);
    }
}
