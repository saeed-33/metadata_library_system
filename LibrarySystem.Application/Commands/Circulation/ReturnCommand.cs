using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Features
{
    public record ReturnCommand(string Barcode) : IRequest<bool>;

public class ReturnCommandHandler : IRequestHandler<ReturnCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public ReturnCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(ReturnCommand request, CancellationToken ct)
    {
        var allCopies = await _unitOfWork.ItemCopies.GetAllAsync();
        var copy = allCopies.FirstOrDefault(c => c.Barcode == request.Barcode);
        if (copy == null) throw new Exception("النسخة غير موجودة.");

        var allRecords = await _unitOfWork.BorrowRecords.GetAllAsync();
        var activeRecord = allRecords.FirstOrDefault(r => r.CopyId == copy.Id && r.ReturnDate == null);
        
        if (activeRecord == null) throw new Exception("لا يوجد سجل إعارة نشط لهذه النسخة.");

        // تحديث السجل
        activeRecord.ReturnDate = DateTime.UtcNow;
        activeRecord.Status = "Returned";

        // إعادة النسخة لتكون متاحة
        copy.Status = 0;

         _unitOfWork.BorrowRecords.Update(activeRecord);
         _unitOfWork.ItemCopies.Update(copy);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
}