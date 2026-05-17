using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands.ItemSets;

public record CreateItemSetCommand(
    string Title,
    string? Description,
    bool IsPublic,
    int? OwnerId
) : IRequest<int>;

public class CreateItemSetCommandHandler : IRequestHandler<CreateItemSetCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateItemSetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateItemSetCommand request, CancellationToken cancellationToken)
    {
        var itemSet = _mapper.Map<ItemSet>(request);

        await _unitOfWork.ItemSets.AddAsync(itemSet);
        await _unitOfWork.SaveChangesAsync();

        return itemSet.Id;
    }
}
