using AutoMapper;
using LibrarySystem.Application.DTOs.ItemSets;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.common;
using LibrarySystem.Domain.Common; // wherever SystemRoles lives
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LibrarySystem.Application.Queries.ItemSets;

public record GetAllItemSetsQuery() : IRequest<IEnumerable<ItemSetResponse>>;

public class GetAllItemSetsQueryHandler : IRequestHandler<GetAllItemSetsQuery, IEnumerable<ItemSetResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllItemSetsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<ItemSetResponse>> Handle(
        GetAllItemSetsQuery request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        bool isAdminOrLibrarian = user != null &&
            (user.IsInRole(SystemRoles.Admin) || user.IsInRole(SystemRoles.Librarian));

        var itemSets = await _unitOfWork.ItemSets.GetAllAsync();

        if (!isAdminOrLibrarian)
        {
            itemSets = itemSets.Where(set => set.IsPublic).ToList();
        }

        return itemSets.Select(itemSet =>
        {
            var response = _mapper.Map<ItemSetResponse>(itemSet);
            response.Items = itemSet.Items.Select(item => new ItemSetItemResponse
            {
                Id = item.Id,
                Type = item.Type,
                TemplateId = item.TemplateId,
                OwnerId = item.OwnerId
            }).ToList();
            return response;
        }).ToList();
    }
}