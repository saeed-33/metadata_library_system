using LibrarySystem.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Items;

public record DeleteItemCommand(int Id) : IRequest<bool>;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        // جلب العنصر مع بياناته التابعة
        var item = await _unitOfWork.Items.GetByIdAsync(request.Id);
        if (item == null) return false;

        // حذف منطقي للقيم المرتبطة (Metadata Values)
        var values = await _unitOfWork.Values.FindAsync(v => v.ResourceId == item.Id);
        foreach (var value in values)
        {
            _unitOfWork.Values.Delete(value);
        }

        // حذف منطقي للنسخ المرتبطة
        var copies = await _unitOfWork.ItemCopies.FindAsync(c => c.ItemId == item.Id);
        foreach (var copy in copies)
        {
            _unitOfWork.ItemCopies.Delete(copy);
        }

        // حذف منطقي للوسائط المرتبطة
        var media = await _unitOfWork.Medias.FindAsync(m => m.ItemId == item.Id);
        foreach (var m in media)
        {
            _unitOfWork.Medias.Delete(m);
        }

        // حذف منطقي للعنصر نفسه
        _unitOfWork.Items.Delete(item);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
