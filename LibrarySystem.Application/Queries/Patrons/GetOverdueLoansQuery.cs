using AutoMapper;
using LibrarySystem.Application.DTOs.BorrowRecords;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Queries.Patrons
{
    public record GetOverdueLoansQuery() : IRequest<List<BorrowRecordResponse>>;

    public class GetOverdueLoansHandler : IRequestHandler<GetOverdueLoansQuery, List<BorrowRecordResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOverdueLoansHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<BorrowRecordResponse>> Handle(GetOverdueLoansQuery request, CancellationToken ct)
        {
            var records = await _unitOfWork.BorrowRecords.GetAllAsync();
            var overdueRecords = records
                .Where(r => r.ReturnDate == null && r.Status == BorrowRecordStatus.Active && r.DueDate < DateTime.UtcNow)
                .ToList();

            // تحديث حالة السجلات المتأخرة فعلياً في قاعدة البيانات
            foreach (var record in overdueRecords)
            {
                record.Status = BorrowRecordStatus.Overdue;
                _unitOfWork.BorrowRecords.Update(record);
            }

            if (overdueRecords.Count > 0)
                await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<List<BorrowRecordResponse>>(overdueRecords);
        }
    }
}
