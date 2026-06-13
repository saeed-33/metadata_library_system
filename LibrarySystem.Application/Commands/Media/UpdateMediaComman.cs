using LibrarySystem.Application.Commands.Items;
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
    public UpdateMediaCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


    public async Task<bool> Handle(UpdateMediaCommand request, CancellationToken cancellationToken)
    {
        var media = (await _unitOfWork.Medias.FindAsync(m => m.Id == request.Id)).FirstOrDefault();
        if (media == null) return false;

        media.StoragePath = request.StoragePath;
        media.FileName = request.FileName;

        // Save media update first
        _unitOfWork.Medias.Update(media);

        // Delete old values
        var oldValues = await _unitOfWork.Values.FindAsync(v => v.ResourceId == media.Id);
        foreach (var v in oldValues)
            _unitOfWork.Values.Delete(v);

        // Add new values with all fields populated
        foreach (var vReq in request.Values)
        {
            await _unitOfWork.Values.AddAsync(new Value
            {
                PropertyId = vReq.PropertyId,
                ValueText = vReq.ValueText,
                ValueUri = vReq.ValueUri,
                ValueResourceId = vReq.ValueResourceId,
                Type = vReq.Type,     // ← was missing
                Language = vReq.Language, // ← was missing
                ResourceId = media.Id
            });
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}