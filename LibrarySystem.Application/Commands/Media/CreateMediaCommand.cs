using AutoMapper;
using LibrarySystem.Application.Commands.Items;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands.Media;

public record CreateMediaCommand(
    int ItemId,
    string StoragePath,
    string FileName,
    List<CreateValueRequest> Values
) : IRequest<int>;

public class CreateMediaCommandHandler : IRequestHandler<CreateMediaCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateMediaCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
    {
        // 1. Create media object
        var media = _mapper.Map<LibrarySystem.Domain.Entities.Media>(request);

        await _unitOfWork.Medias.AddAsync(media);

        // 2. Save media first to get a real Id from the database
        await _unitOfWork.SaveChangesAsync();

        // 3. Now media.Id is populated — safe to use as FK for Values
        foreach (var vReq in request.Values)
        {
            var value = _mapper.Map<Value>(vReq);
            value.ResourceId = media.Id;  // ← use Id directly, not navigation property
            await _unitOfWork.Values.AddAsync(value);
        }

        await _unitOfWork.SaveChangesAsync();
        return media.Id;
    }
}
