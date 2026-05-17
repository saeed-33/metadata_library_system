using AutoMapper;
using LibrarySystem.Application.DTOs.Properties;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Properties;

public record GetPropertyByIdQuery(int Id) : IRequest<PropertyResponse?>;

public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, PropertyResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPropertyByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PropertyResponse?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        // نستخدم FindAsync لجلب الخاصية مع القاموس التابع لها (Include)
        var properties = await _unitOfWork.Properties.FindAsync(
            p => p.Id == request.Id,
            p => p.Vocabulary!
        );

        var property = properties.FirstOrDefault();
        return property == null ? null : _mapper.Map<PropertyResponse>(property);
    }
}