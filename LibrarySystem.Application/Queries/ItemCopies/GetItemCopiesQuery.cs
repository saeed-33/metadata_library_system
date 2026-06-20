using AutoMapper;
using LibrarySystem.Application.DTOs.ItemCopies;
using LibrarySystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Queries.ItemCopies;

public record GetItemCopiesQuery(int ItemId) : IRequest<List<ItemCopyResponse>>;

public class GetItemCopiesHandler : IRequestHandler<GetItemCopiesQuery, List<ItemCopyResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetItemCopiesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ItemCopyResponse>> Handle(GetItemCopiesQuery request, CancellationToken ct)
    {
        var copies = await _unitOfWork.ItemCopies.GetAllAsync();
        return _mapper.Map<List<ItemCopyResponse>>(copies.Where(c => c.ItemId == request.ItemId).ToList());
    }
}
