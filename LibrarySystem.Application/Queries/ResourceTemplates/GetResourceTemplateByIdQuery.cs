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

    public async Task<ResourceTemplateResponse?> Handle(GetResourceTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        // نستخدم FindAsync مع Include لجلب الـ TemplateProperties والـ Property نفسها
        var templates = await _unitOfWork.ResourceTemplates.FindAsync(
            t => t.Id == request.Id,
            t => t.TemplateProperties // جلب جدول الربط
        );

        var template = templates.FirstOrDefault();
        return _mapper.Map<ResourceTemplateResponse>(template);
    }
}