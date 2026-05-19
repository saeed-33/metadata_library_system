using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands.ResourceTemplates;

// الطلب الذي سيصل من الفرونت اند (React)
public record UpdateTemplatePropertiesCommand(
    int TemplateId,
    List<TemplatePropertyRequest> Properties
) : IRequest<bool>;

// كلاس فرعي لنقل بيانات الربط
public record TemplatePropertyRequest(
    int PropertyId,
    bool IsRequired,
    int DisplayOrder,
    string? AlternateLabel
);

public class UpdateTemplatePropertiesCommandHandler : IRequestHandler<UpdateTemplatePropertiesCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTemplatePropertiesCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateTemplatePropertiesCommand request, CancellationToken cancellationToken)
    {
        // 1. التأكد من وجود القالب أولاً
        var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(request.TemplateId);
        if (template == null) return false;

        // 2. مسح العلاقات القديمة (Logic: Reset then Insert)
        // هذا الأسلوب هو الأفضل لضمان مزامنة قائمة الخصائص مع ما يراه المستخدم في الواجهة
        var currentLinks = await _unitOfWork.TemplateProperties.FindAsync(tp => tp.TemplateId == request.TemplateId);
        foreach (var link in currentLinks)
        {
            _unitOfWork.TemplateProperties.Delete(link);
        }

        // 3. إضافة العلاقات الجديدة بناءً على الطلب
        foreach (var propReq in request.Properties)
        {
            var newRelation = new TemplateProperty
            {
                TemplateId = request.TemplateId,
                PropertyId = propReq.PropertyId,
                IsRequired = propReq.IsRequired,
                DisplayOrder = propReq.DisplayOrder,
                AlternateLabel = propReq.AlternateLabel
            };
            await _unitOfWork.TemplateProperties.AddAsync(newRelation);
        }

        // 4. الحفظ في قاعدة البيانات
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}