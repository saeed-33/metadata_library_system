using AutoMapper;
using LibrarySystem.Application.DTOs.ResourceTemplates;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.ResourceTemplates;

public record GetAllResourceTemplatesQuery() : IRequest<IEnumerable<ResourceTemplateResponse>>;

public class GetAllResourceTemplatesQueryHandler : IRequestHandler<GetAllResourceTemplatesQuery, IEnumerable<ResourceTemplateResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllResourceTemplatesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResourceTemplateResponse>> Handle(
    GetAllResourceTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.ResourceTemplates.FindAsync(
            t => true,
            t => t.TemplateProperties
        );

        // Collect all property ids across all templates
        var propertyIds = templates
            .SelectMany(t => t.TemplateProperties)
            .Select(tp => tp.PropertyId)
            .Distinct()
            .ToList();

        // Single DB call to get all needed properties
        var properties = await _unitOfWork.Properties.FindAsync(
            p => propertyIds.Contains(p.Id)
        );

        var propertyLabels = properties.ToDictionary(p => p.Id, p => p.Label);

        var response = _mapper.Map<IEnumerable<ResourceTemplateResponse>>(templates).ToList();

        foreach (var template in response)
        {
            foreach (var prop in template.Properties)
            {
                prop.PropertyLabel = propertyLabels.TryGetValue(prop.PropertyId, out var label)
                    ? label
                    : string.Empty;
            }
        }

        return response;
    }
}