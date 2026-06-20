using AutoMapper;
using LibrarySystem.Application.DTOs.BorrowRecords;
using LibrarySystem.Application.DTOs.Circulation;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Circulation;

public record GetBorrowHistoryQuery(int? ItemId = null, int? PatronId = null) : IRequest<List<BorrowRecordAdminResponse>>;

public class GetBorrowHistoryHandler : IRequestHandler<GetBorrowHistoryQuery, List<BorrowRecordAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBorrowHistoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<BorrowRecordAdminResponse>> Handle(GetBorrowHistoryQuery request, CancellationToken ct)
    {
        var records = await _unitOfWork.BorrowRecords.GetAllAsync();
        
        // فلترة البيانات بناءً على طلب الـ UI
        if (request.ItemId.HasValue)
        {
            var copies = await _unitOfWork.ItemCopies.GetAllAsync();
            var itemCopyIds = copies.Where(c => c.ItemId == request.ItemId.Value).Select(c => c.Id);
            records = records.Where(r => itemCopyIds.Contains(r.CopyId));
        }

        if (request.PatronId.HasValue)
        {
            records = records.Where(r => r.PatronId == request.PatronId.Value);
        }

        return _mapper.Map<List<BorrowRecordAdminResponse>>(records.OrderByDescending(r => r.BorrowDate).ToList());
    }
}