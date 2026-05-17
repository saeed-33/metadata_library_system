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
        var media = (await _unitOfWork.Medias.FindAsync(m => m.Id == request.Id, m => m.Values)).FirstOrDefault();
        if (media == null) return false;

        media.StoragePath = request.StoragePath;
        media.FileName = request.FileName;

        // تحديث القيم (نتبع نفس منطق الـ Item: مسح القديم وإضافة الجديد لضمان التكامل)
        var oldValues = await _unitOfWork.Values.FindAsync(v => v.ResourceId == media.Id);
        foreach (var v in oldValues) _unitOfWork.Values.Delete(v);

        foreach (var vReq in request.Values)
        {
            await _unitOfWork.Values.AddAsync(new Value
            {
                PropertyId = vReq.PropertyId,
                ValueText = vReq.ValueText,
                ResourceId = media.Id
            });
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}