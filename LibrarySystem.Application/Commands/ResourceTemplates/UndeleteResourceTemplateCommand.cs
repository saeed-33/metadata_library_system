using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.ResourceTemplates;

public record UndeleteResourceTemplateCommand(int Id) : IRequest<bool>;

public class UndeleteResourceTemplateCommandHandler : IRequestHandler<UndeleteResourceTemplateCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeleteResourceTemplateCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeleteResourceTemplateCommand request, CancellationToken cancellationToken)
    {
        // 1. جلب القالب مع المحذوفين (بما أنه Soft Delete)
        var templates = await _unitOfWork.ResourceTemplates.FindWithDeletedAsync(t => t.Id == request.Id);
        var template = templates.FirstOrDefault();

        if (template == null)
            return false;

        // 2. Restore (undo soft delete)
        template.IsDeleted = false;
        template.DeletedAt = null;

        // 3. ملاحظة: علاقات TemplateProperties تم حذفها نهائياً (Hard Delete) في Delete,
        //    لذا لا يمكن استعادتها. نقوم فقط باستعادة القالب نفسه.

        _unitOfWork.ResourceTemplates.Update(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}