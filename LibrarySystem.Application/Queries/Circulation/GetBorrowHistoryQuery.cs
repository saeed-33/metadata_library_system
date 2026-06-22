using AutoMapper;
using LibrarySystem.Application.DTOs.BorrowRecords;
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
        // تحميل العلاقات المطلوبة لعرض الباركود واسم المستعير
        var records = await _unitOfWork.BorrowRecords.FindAsync(
            r => true,
            r => r.Copy.Item,
            r => r.Patron);

        // فلترة البيانات بناءً على طلب الـ UI
        if (request.ItemId.HasValue)
        {
            records = records.Where(r => r.Copy != null && r.Copy.ItemId == request.ItemId.Value);
        }

        if (request.PatronId.HasValue)
        {
            records = records.Where(r => r.PatronId == request.PatronId.Value);
        }

        return _mapper.Map<List<BorrowRecordAdminResponse>>(records.OrderByDescending(r => r.BorrowDate).ToList());
    }
}
