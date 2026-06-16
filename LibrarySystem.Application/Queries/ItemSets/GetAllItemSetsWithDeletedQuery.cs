using AutoMapper;
using LibrarySystem.Application.DTOs.ItemSets;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.ItemSets;

public record GetAllItemSetsWithDeletedQuery() : IRequest<IEnumerable<ItemSetAdminResponse>>;

public class GetAllItemSetsWithDeletedQueryHandler : IRequestHandler<GetAllItemSetsWithDeletedQuery, IEnumerable<ItemSetAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllItemSetsWithDeletedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ItemSetAdminResponse>> Handle(
        GetAllItemSetsWithDeletedQuery request, CancellationToken cancellationToken)
    {
        // جلب كل المجموعات بما فيها المحذوفة
        var itemSets = await _unitOfWork.ItemSets.GetAllWithDeletedAsync();
                                                  
        return itemSets.Select(itemSet =>
        {
            // استخدم المابر للتحويل إلى Admin Response
            var response = _mapper.Map<ItemSetAdminResponse>(itemSet);

            // Inject items manually
            response.Items = itemSet.Items.Select(item => new ItemSetItemAdminResponse
            {
                Id = item.Id,
                Type = item.Type,
                TemplateId = item.TemplateId,
                OwnerId = item.OwnerId,
                IsDeleted = item.IsDeleted // أضفنا خاصية الإدمن أيضاً

                // إذا كان لدى ItemSetItemResponse خاصية IsDeleted، قم بتعيينها هنا: item.IsDeleted
            }).ToList();

            return response;
        }).ToList();
    }
}