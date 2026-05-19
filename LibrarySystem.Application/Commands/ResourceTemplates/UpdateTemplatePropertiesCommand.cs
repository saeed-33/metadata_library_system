using AutoMapper; // أضف هذا السطر
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
    private readonly IMapper _mapper; // حقن المابير

    public UpdateTemplatePropertiesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateTemplatePropertiesCommand request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(request.TemplateId);
        if (template == null) return false;

        // مسح العلاقات القديمة
        var currentLinks = await _unitOfWork.TemplateProperties.FindAsync(tp => tp.TemplateId == request.TemplateId);
        foreach (var link in currentLinks)
        {
            _unitOfWork.TemplateProperties.Delete(link);
        }

        foreach (var propReq in request.Properties)
        {
            var newRelation = _mapper.Map<TemplateProperty>(propReq);

            newRelation.TemplateId = request.TemplateId;

            await _unitOfWork.TemplateProperties.AddAsync(newRelation);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}