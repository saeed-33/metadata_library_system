using LibrarySystem.Application.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibrarySystem.Application.DTOs.Circulation;


namespace LibrarySystem.Application.Commands.Circulation
{

    public class ReturnCommandHandler : IRequestHandler<ReturnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReturnCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ReturnCommand request, CancellationToken ct)
        {
            // 1. Fetch the copy safely (Validation guarantees it exists)
            var copies = await _unitOfWork.ItemCopies.FindAsync(c => c.Barcode == request.Barcode);
            var copy = copies.First();

            // 2. Fetch the active borrow record safely (Validation guarantees it exists)
            var records = await _unitOfWork.BorrowRecords.FindAsync(r => r.CopyId == copy.Id && r.ReturnDate == null);
            var activeRecord = records.First();

            // 3. Update the record
            activeRecord.ReturnDate = DateTime.UtcNow;
            activeRecord.Status = "Returned";

            // 4. Reset the copy status to Available (0)
            copy.Status = 0;

            // 5. Update and Save
            _unitOfWork.BorrowRecords.Update(activeRecord);
            _unitOfWork.ItemCopies.Update(copy);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}