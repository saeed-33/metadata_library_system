using AutoMapper;
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
            // 1. التحقق من وجود المستعير
            var patron = await _unitOfWork.Patrons.GetByIdAsync(request.PatronId);
            if (patron == null)
                throw new InvalidOperationException("المستعير غير موجود.");

            // 2. البحث عن النسخة بواسطة الباركود
            var allCopies = await _unitOfWork.ItemCopies.GetAllAsync();
            var copy = allCopies.FirstOrDefault(c => c.Barcode == request.Barcode);

            if (copy == null)
                throw new InvalidOperationException("النسخة غير موجودة.");

            // 3. التحقق من أن النسخة متاحة
            if (copy.Status != ItemCopyStatus.Available)
                throw new InvalidOperationException("هذه النسخة غير متاحة حالياً (معارة أو تحت الصيانة).");

            // 4. التحقق من سياسة الإعارة للقالب
            var item = await _unitOfWork.Items.GetByIdAsync(copy.ItemId);
            if (item == null)
                throw new InvalidOperationException("الكتاب غير موجود.");

            if (!item.TemplateId.HasValue)
                throw new InvalidOperationException("هذا الكتاب غير مرتبط بقالب مصادر.");

            var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(item.TemplateId.Value);
            if (template == null || !template.IsBorrowable)
                throw new InvalidOperationException("هذا الصنف مخصص للمراجع فقط ولا يُسمح بإعارته.");

            // 5. حساب تاريخ الاستحقاق
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

            // 6. إنشاء سجل الإعارة
            var borrowRecord = _mapper.Map<BorrowRecord>(request);
            borrowRecord.CopyId = copy.Id;
            borrowRecord.BorrowDate = DateTime.UtcNow;
            borrowRecord.DueDate = dueDate;
            borrowRecord.Status = BorrowRecordStatus.Active;
            borrowRecord.OriginalCopyStatus = copy.Status; // حفظ الحالة الأصلية للاستعادة عند الإرجاع

            // 7. تحديث حالة النسخة إلى "معارة"
            copy.Status = ItemCopyStatus.Borrowed;

            await _unitOfWork.BorrowRecords.AddAsync(borrowRecord);
            _unitOfWork.ItemCopies.Update(copy);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}