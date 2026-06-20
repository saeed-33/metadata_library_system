using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands.ResourceTemplates;

public record UpdateTemplatePropertiesCommand(
    int TemplateId,
    List<TemplatePropertyRequest> Properties
) : IRequest<bool>;

public record TemplatePropertyRequest(
    int PropertyId,
    bool IsRequired,
    int DisplayOrder,
    string? AlternateLabel
);

public class UpdateTemplatePropertiesCommandHandler
    : IRequestHandler<UpdateTemplatePropertiesCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateTemplatePropertiesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(
        UpdateTemplatePropertiesCommand request,
        CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.ResourceTemplates
            .GetByIdAsync(request.TemplateId);
        if (template == null) return false;

        // Get current active links
        var currentLinks = (await _unitOfWork.TemplateProperties
            .FindAsync(tp => tp.TemplateId == request.TemplateId))
            .ToList();

        foreach (var propReq in request.Properties)
        {
            // Try to restore/update existing row (active or soft-deleted)
            var restored = await _unitOfWork.RestoreOrUpdateTemplatePropertyAsync(
                request.TemplateId,
                propReq.PropertyId,
                propReq.IsRequired,
                propReq.DisplayOrder,
                propReq.AlternateLabel);

            if (!restored)
            {
                // Row doesn't exist at all — truly new, safe to insert
                var newRelation = _mapper.Map<TemplateProperty>(propReq);
                newRelation.TemplateId = request.TemplateId;
                await _unitOfWork.TemplateProperties.AddAsync(newRelation);
            }

            // Mark as handled
            var handled = currentLinks
                .FirstOrDefault(tp => tp.PropertyId == propReq.PropertyId);
            if (handled != null)
                currentLinks.Remove(handled);
        }

        // Soft delete whatever was not in the new request
        foreach (var removed in currentLinks)
            _unitOfWork.TemplateProperties.Delete(removed);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
