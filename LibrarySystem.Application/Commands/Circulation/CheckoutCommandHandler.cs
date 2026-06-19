using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Features
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            // 1. البحث عن النسخة بواسطة الباركود
            // ملاحظة: قد تحتاج لإضافة ميثود GetByBarcode في المستودع أو استخدام Find
            var allCopies = await _unitOfWork.ItemCopies.GetAllAsync();
            var copy = allCopies.FirstOrDefault(c => c.Barcode == request.Barcode);

            if (copy == null) throw new Exception("النسخة غير موجودة.");

            // 2. التحقق من حالة النسخة (يجب أن تكون 0: Available)
            if (copy.Status != 0) throw new Exception("هذه النسخة غير متاحة حالياً (معارة أو تحت الصيانة).");

            // 3. التحقق من سياسة الإعارة للقالب
            var item = await _unitOfWork.Items.GetByIdAsync(copy.ItemId);

            // التحقق من أن الكتاب موجود أولاً
            if (item == null) throw new Exception("الكتاب غير موجود.");

            // التحقق من أن القالب مرتبط بالكتاب قبل استخدامه
            if (!item.TemplateId.HasValue) throw new Exception("هذا الكتاب غير مرتبط بقالب مصادر.");

            // الآن نرسل القيمة بعد التأكد أنها ليست Null باستخدام .Value
            var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(item.TemplateId.Value);

            if (template == null || !template.IsBorrowable)
                throw new Exception("هذا الصنف مخصص للمراجع فقط ولا يُسمح بإعارته.");

            // 4. الحساب الذكي لتاريخ الاستحقاق (Due Date)
            DateTime dueDate;
            if (request.CustomDueDate.HasValue)
            {
                dueDate = request.CustomDueDate.Value;
            }
            else if (template.DefaultBorrowDays.HasValue)
            {
                dueDate = DateTime.UtcNow.AddDays(template.DefaultBorrowDays.Value);
            }
            else
            {
                var settings = await _unitOfWork.SystemSettings.GetAllAsync();
                var globalDays = settings.FirstOrDefault(s => s.Key == "GlobalBorrowDays")?.Value;
                int days = int.TryParse(globalDays, out var d) ? d : 14; // الافتراضي 14 يوم
                dueDate = DateTime.UtcNow.AddDays(days);
            }

            // 5. إنشاء سجل الإعارة
            var borrowRecord = new BorrowRecord
            {
                CopyId = copy.Id,
                PatronId = request.PatronId,
                BorrowDate = DateTime.UtcNow,
                DueDate = dueDate,
                Status = "Active"
            };

            // 6. تحديث حالة النسخة إلى "معارة" (1)
            copy.Status = 1;

            await _unitOfWork.BorrowRecords.AddAsync(borrowRecord);
             _unitOfWork.ItemCopies.Update(copy);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}