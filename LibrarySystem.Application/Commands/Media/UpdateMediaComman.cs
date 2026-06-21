using AutoMapper;
using LibrarySystem.Application.DTOs.Values;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands.Media;

public record UpdateMediaCommand(
    int Id,
    string StoragePath,
    string FileName,
    List<CreateValueRequest> Values
) : IRequest<bool>;

public class UpdateMediaCommandHandler : IRequestHandler<UpdateMediaCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateMediaCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<bool> Handle(UpdateMediaCommand request, CancellationToken cancellationToken)
    {
        var media = (await _unitOfWork.Medias.FindAsync(m => m.Id == request.Id)).FirstOrDefault();
        if (media == null) return false;

        _mapper.Map(request, media);

        // Save media update first
        _unitOfWork.Medias.Update(media);

        // Delete old values
        var oldValues = await _unitOfWork.Values.FindAsync(v => v.ResourceId == media.Id);
        foreach (var v in oldValues)
            _unitOfWork.Values.Delete(v);

        // Add new values with all fields populated
        foreach (var vReq in request.Values)
        {
            var value = _mapper.Map<Value>(vReq);
            value.ResourceId = media.Id;
            await _unitOfWork.Values.AddAsync(value);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
