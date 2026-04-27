using LibrarySystem.Application.Commands.Items;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

public record UpdateItemCommand(
    int Id,
    int? TemplateId,
    List<CreateValueRequest> Values // القيم الجديدة/المعدلة
) : IRequest<bool>;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        // 1. جلب الـ Item مع قيمه الحالية
        var item = (await _unitOfWork.Items.FindAsync(i => i.Id == request.Id, i => i.Values)).FirstOrDefault();
        if (item == null) return false;

        // 2. تحديث بيانات الـ Item الأساسية
        item.TemplateId = request.TemplateId;

        // 3. تحديث القيم (Logic: مسح القديم وإضافة الجديد - أسهل وأضمن طريقة في Metadata)
        var oldValues = await _unitOfWork.Values.FindAsync(v => v.ResourceId == item.Id);
        foreach (var oldVal in oldValues)
        {
            _unitOfWork.Values.Delete(oldVal);
        }

        // 4. إضافة القيم الجديدة المرسلة في الطلب
        foreach (var vReq in request.Values)
        {
            var newValue = new Value
            {
                PropertyId = vReq.PropertyId,
                ValueText = vReq.ValueText,
                Type = vReq.Type,
                Language = vReq.Language,
                ResourceId = item.Id // ربطها بالـ Item الحالي
            };
            await _unitOfWork.Values.AddAsync(newValue);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}