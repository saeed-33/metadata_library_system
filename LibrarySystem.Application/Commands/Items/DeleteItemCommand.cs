using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Items;

public record DeleteItemCommand(int Id) : IRequest<bool>;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        // 1. جلب العنصر (نستخدم المستودع الخاص بالـ Items)
        var item = await _unitOfWork.Items.GetByIdAsync(request.Id);

        if (item == null) return false;

        // 2. تنفيذ الحذف
        // ملاحظة للمناقشة: بما أن Item يرث من Resource الذي يرث من BasePersistenceModel
        // فإن استدعاء Delete سيقوم تلقائياً بعمل Soft Delete وتحديث حقل IsDeleted
        _unitOfWork.Items.Delete(item);

        // 3. ماذا عن القيم المرتبطة (Values)؟
        // في التصميم الاحترافي، إذا كان المورد محذوفاً (IsDeleted=true)، 
        // فإن الاستعلامات (Queries) ستتجاهل قيمه تلقائياً بفضل الـ Global Query Filter

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}