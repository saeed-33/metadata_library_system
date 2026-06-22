using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibrarySystem.Application.DTOs.Circulation;

namespace LibrarySystem.Application.Commands.Circulation
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CheckoutCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            // 1. Fetch data safely 
            // (FluentValidation already guaranteed that the Copy, Item, and Template exist and are valid)
            var copies = await _unitOfWork.ItemCopies.FindAsync(c => c.Barcode == request.Barcode);
            var copy = copies.First();

            var item = await _unitOfWork.Items.GetByIdAsync(copy.ItemId);
            var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(item!.TemplateId!.Value);

            // 2. Smart Due Date Calculation
            DateTime dueDate;
            if (request.CustomDueDate.HasValue)
            {
                dueDate = request.CustomDueDate.Value;
            }
            else if (template!.DefaultBorrowDays.HasValue)
            {
                dueDate = DateTime.UtcNow.AddDays(template.DefaultBorrowDays.Value);
            }
            else
            {
                var settings = await _unitOfWork.SystemSettings.GetAllAsync();
                var globalDays = settings.FirstOrDefault(s => s.Key == "GlobalBorrowDays")?.Value;
                int days = int.TryParse(globalDays, out var d) ? d : 14; // Default 14 days
                dueDate = DateTime.UtcNow.AddDays(days);
            }

            // 3. Create Borrow Record
            var borrowRecord = _mapper.Map<BorrowRecord>(request);
            borrowRecord.CopyId = copy.Id;
            borrowRecord.BorrowDate = DateTime.UtcNow;
            borrowRecord.DueDate = dueDate;
            borrowRecord.Status = "Active";

            // 4. Update Copy Status to "Borrowed" (1)
            copy.Status = 1;

            await _unitOfWork.BorrowRecords.AddAsync(borrowRecord);
            _unitOfWork.ItemCopies.Update(copy);

            // 5. Save changes
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}