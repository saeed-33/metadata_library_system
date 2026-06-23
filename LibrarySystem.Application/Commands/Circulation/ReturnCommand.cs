using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Domain.Enums;
using MediatR;
using System;
using System.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibrarySystem.Application.DTOs.Circulation;


namespace LibrarySystem.Application.Commands.Circulation
{

    public class ReturnCommandHandler : IRequestHandler<ReturnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReturnCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(ReturnCommand request, CancellationToken ct)
        {
            var allCopies = await _unitOfWork.ItemCopies.GetAllAsync();
            var copy = allCopies.FirstOrDefault(c => c.Barcode == request.Barcode);
            if (copy == null)
                throw new InvalidOperationException("النسخة غير موجودة.");

            var allRecords = await _unitOfWork.BorrowRecords.GetAllAsync();
            var activeRecord = allRecords
                .FirstOrDefault(r => r.CopyId == copy.Id && r.ReturnDate == null && r.Status == BorrowRecordStatus.Active);

            if (activeRecord == null)
                throw new InvalidOperationException("لا يوجد سجل إعارة نشط لهذه النسخة.");

            // تحديث السجل
            activeRecord.ReturnDate = DateTime.UtcNow;
            activeRecord.Status = BorrowRecordStatus.Returned;

            // استعادة الحالة الأصلية للنسخة (متاح، مرجعي فقط، صيانة...)
            copy.Status = activeRecord.OriginalCopyStatus;

            _unitOfWork.BorrowRecords.Update(activeRecord);
            _unitOfWork.ItemCopies.Update(copy);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
            return true;
        }
    }
}
