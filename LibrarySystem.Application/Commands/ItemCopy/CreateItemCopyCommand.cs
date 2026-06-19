using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Features
{
    public record CreateItemCopyCommand(int ItemId, string Barcode, string? Notes) : IRequest<int>;

    public class CreateItemCopyHandler : IRequestHandler<CreateItemCopyCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateItemCopyHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<int> Handle(CreateItemCopyCommand request, CancellationToken ct)
        {
            // التحقق من الباركود
            var copies = await _unitOfWork.ItemCopies.GetAllAsync();
            if (copies.Any(c => c.Barcode == request.Barcode))
                throw new Exception("هذا الباركود مستخدم مسبقاً.");

            // التحقق من الكتاب والقالب
            var item = await _unitOfWork.Items.GetByIdAsync(request.ItemId);
            // التحقق من أن الكتاب موجود أولاً
            if (item == null) throw new Exception("الكتاب غير موجود.");

            // التحقق من أن القالب مرتبط بالكتاب قبل استخدامه
            if (!item.TemplateId.HasValue) throw new Exception("هذا الكتاب غير مرتبط بقالب مصادر.");

            // الآن نرسل القيمة بعد التأكد أنها ليست Null باستخدام .Value
            var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(item.TemplateId.Value);

            var copy = new ItemCopy
            {
                ItemId = request.ItemId,
                Barcode = request.Barcode,
                Notes = request.Notes,
                // إذا كان القالب لا يسمح بالإعارة، النسخة تكون مراجع فقط (2) وإلا متاحة (0)
                Status = (template != null && !template.IsBorrowable) ? 2 : 0
            };

            

            await _unitOfWork.ItemCopies.AddAsync(copy);
            await _unitOfWork.SaveChangesAsync();
            return copy.Id;
        }
    }
}