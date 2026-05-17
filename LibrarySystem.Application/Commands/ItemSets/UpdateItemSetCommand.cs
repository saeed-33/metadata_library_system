using AutoMapper;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.ItemSets;

public record UpdateItemSetCommand(
    int Id,
    string Title,
    string? Description,
    bool IsPublic,
    int? OwnerId
) : IRequest<bool>;

public class UpdateItemSetCommandHandler : IRequestHandler<UpdateItemSetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateItemSetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateItemSetCommand request, CancellationToken cancellationToken)
    {
        var itemSet = await _unitOfWork.ItemSets.GetByIdAsync(request.Id);
        if (itemSet == null) return false;

        _mapper.Map(request, itemSet);
        var updated = await _unitOfWork.ItemSets.UpdateAsync(itemSet);
        if (!updated) return false;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
