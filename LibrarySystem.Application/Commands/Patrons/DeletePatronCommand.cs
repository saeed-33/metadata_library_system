using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Patrons;

public record DeletePatronCommand(int Id) : IRequest<bool>;

public class DeletePatronHandler : IRequestHandler<DeletePatronCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeletePatronHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeletePatronCommand request, CancellationToken ct)
    {
        var patron = await _unitOfWork.Patrons.GetByIdAsync(request.Id);
        if (patron == null) return false;

        // التحقق مما إذا كان لديه إعارات نشطة قبل الحذف
        var records = await _unitOfWork.BorrowRecords.GetAllAsync();
        if (records.Any(r => r.PatronId == request.Id && r.ReturnDate == null))
            throw new Exception("لا يمكن حذف المستعير لأن لديه كتب لم يتم إرجاعها بعد.");

         _unitOfWork.Patrons.Delete(patron);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}