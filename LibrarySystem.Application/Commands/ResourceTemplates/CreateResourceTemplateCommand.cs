using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;
using AutoMapper;

namespace LibrarySystem.Application.Commands.ResourceTemplates;

public record CreateResourceTemplateCommand(
    string Label,
    string? Description,
    bool IsBorrowable = true,
    int? DefaultBorrowDays = null
) : IRequest<int>;

public class CreateResourceTemplateCommandHandler : IRequestHandler<CreateResourceTemplateCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateResourceTemplateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateResourceTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = _mapper.Map<ResourceTemplate>(request);
        await _unitOfWork.ResourceTemplates.AddAsync(template);
        await _unitOfWork.SaveChangesAsync();
        return template.Id;
    }
}