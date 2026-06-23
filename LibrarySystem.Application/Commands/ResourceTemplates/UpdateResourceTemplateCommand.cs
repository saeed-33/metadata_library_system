using AutoMapper;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.ResourceTemplates;

public record UpdateResourceTemplateCommand(
    int Id,
    string Label,
    string? Description,
    bool IsBorrowable,
    int? DefaultBorrowDays
) : IRequest<bool>;

public class UpdateResourceTemplateCommandHandler : IRequestHandler<UpdateResourceTemplateCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateResourceTemplateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateResourceTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(request.Id);
        if (template == null) return false;

        _mapper.Map(request, template);
        _unitOfWork.ResourceTemplates.Update(template);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}