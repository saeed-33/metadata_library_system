using LibrarySystem.Application.Commands.Items;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands.Media;

public record CreateMediaCommand(
    int ItemId,
    string StoragePath,
    string FileName,
    List<CreateValueRequest> Values // قيم وصفية للملف
) : IRequest<int>;

public class CreateMediaCommandHandler : IRequestHandler<CreateMediaCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMediaCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<int> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
    {
        // 1. إنشاء كائن الـ Media
        var media = new LibrarySystem.Domain.Entities.Media
        {
            ItemId = request.ItemId,
            StoragePath = request.StoragePath,
            FileName = request.FileName
        };

        await _unitOfWork.Medias.AddAsync(media);

        // 2. إضافة القيم الوصفية المرتبطة بالملف
        foreach (var vReq in request.Values)
        {
            var value = new Value
            {
                PropertyId = vReq.PropertyId,
                ValueText = vReq.ValueText,
                Type = vReq.Type,
                Language = vReq.Language,
                Resource = media // ربط القيمة بملف الميديا
            };
            await _unitOfWork.Values.AddAsync(value);
        }

        await _unitOfWork.SaveChangesAsync();
        return media.Id;
    }
}