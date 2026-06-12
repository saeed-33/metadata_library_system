using AutoMapper;
using LibrarySystem.Application.DTOs.ResourceTemplates;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.ResourceTemplates;

public record GetResourceTemplateByIdQuery(int Id) : IRequest<ResourceTemplateResponse?>;

public class GetResourceTemplateByIdQueryHandler : IRequestHandler<GetResourceTemplateByIdQuery, ResourceTemplateResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetResourceTemplateByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResourceTemplateResponse?> Handle(
    GetResourceTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.ResourceTemplates.FindAsync(
            t => t.Id == request.Id,
            t => t.TemplateProperties
        );

        var template = templates.FirstOrDefault();
        if (template == null) return null;

        // Get all property ids from the template
        var propertyIds = template.TemplateProperties.Select(tp => tp.PropertyId).ToList();

        // Fetch the actual Property entities to get their labels
        var properties = await _unitOfWork.Properties.FindAsync(
            p => propertyIds.Contains(p.Id)
        );

        // Build a lookup dictionary: propertyId → label
        var propertyLabels = properties.ToDictionary(p => p.Id, p => p.Label);

        // Map to response and inject labels manually
        var response = _mapper.Map<ResourceTemplateResponse>(template);
        foreach (var prop in response.Properties)
        {
            prop.PropertyLabel = propertyLabels.TryGetValue(prop.PropertyId, out var label)
                ? label
                : string.Empty;
        }

        return response;
    }
}