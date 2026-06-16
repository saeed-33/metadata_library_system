using AutoMapper;
using LibrarySystem.Application.DTOs.ResourceTemplates;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.ResourceTemplates;

public record GetAllResourceTemplatesWithDeletedQuery() : IRequest<IEnumerable<ResourceTemplateAdminResponse>>;

public class GetAllResourceTemplatesWithDeletedQueryHandler : IRequestHandler<GetAllResourceTemplatesWithDeletedQuery, IEnumerable<ResourceTemplateAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllResourceTemplatesWithDeletedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResourceTemplateAdminResponse>> Handle(
    GetAllResourceTemplatesWithDeletedQuery request, CancellationToken cancellationToken)
    {
        // 1. جلب التمبلت المحذوفة والغير محذوفة
        var templates = await _unitOfWork.ResourceTemplates.FindWithDeletedAsync(
            t => true,
            t => t.TemplateProperties
        );

        var propertyIds = templates
            .SelectMany(t => t.TemplateProperties)
            .Select(tp => tp.PropertyId)
            .Distinct()
            .ToList();

        // 2. استخدام FindWithDeletedAsync هنا ضروري جداً! 
        // حتى لو كان الـ Property محذوفاً، نريد أن نعرض اسمه للإدمن
        var properties = await _unitOfWork.Properties.FindWithDeletedAsync(
            p => propertyIds.Contains(p.Id)
        );

        var propertyLabels = properties.ToDictionary(p => p.Id, p => p.Label);

        // 3. التحويل إلى كلاسات الإدمن
        var response = _mapper.Map<IEnumerable<ResourceTemplateAdminResponse>>(templates).ToList();

        foreach (var template in response)
        {
            foreach (var prop in template.Properties) // Properties هنا من نوع TemplatePropertyAdminResponse
            {
                prop.PropertyLabel = propertyLabels.TryGetValue(prop.PropertyId, out var label)
                    ? label
                    : string.Empty;
            }
        }

        return response;
    }
}