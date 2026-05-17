using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.ResourceTemplates;

public record DeleteResourceTemplateCommand(int Id) : IRequest<DeleteResult>;

// سننشئ نوع إرجاع مخصص لنخبر الـ Controller بنوع الخطأ
public record DeleteResult(bool IsSuccess, string Message = "");

public class DeleteResourceTemplateCommandHandler : IRequestHandler<DeleteResourceTemplateCommand, DeleteResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResourceTemplateCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<DeleteResult> Handle(DeleteResourceTemplateCommand request, CancellationToken cancellationToken)
    {
        // 1. جلب القالب
        var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(request.Id);
        if (template == null)
            return new DeleteResult(false, "القالب غير موجود.");

        // 2. التحقق من الاستخدام (المنطق الاحترافي)
        var isUsed = (await _unitOfWork.Items.FindAsync(i => i.TemplateId == request.Id)).Any();

        if (isUsed)
        {
            return new DeleteResult(false, "لا يمكن حذف القالب لأنه مرتبط بكتب (Items) موجودة حالياً. يجب تغيير قالب الكتب أولاً.");
        }

        // 3. حذف علاقات القالب بالخصائص (TemplateProperties)
        // بما أن TemplateProperty ليس له Repository مستقل، يمكننا الوصول له من خلال Context 
        // أو الأفضل: نجلب الـ Template مع خصائصه ونمسحها
        var properties = await _unitOfWork.TemplateProperties.FindAsync(tp => tp.TemplateId == request.Id);
        foreach (var prop in properties)
        {
            _unitOfWork.TemplateProperties.Delete(prop);
            // ملاحظة: بما أنها لا ترث من BasePersistenceModel ستمسح نهائياً من قاعدة البيانات (Hard Delete)
            // وهذا منطقي لأنها مجرد "علاقة ربط" وليست بيانات حقيقية
        }

        // 4. حذف القالب (Soft Delete)
        _unitOfWork.ResourceTemplates.Delete(template);

        await _unitOfWork.SaveChangesAsync();

        return new DeleteResult(true, "تم حذف القالب وعلاقاته بنجاح.");
    }
}