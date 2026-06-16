using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Items;

public record UndeletItemCommand(int Id) : IRequest<bool>;

public class UndeletItemCommandHandler : IRequestHandler<UndeletItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeletItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeletItemCommand request, CancellationToken cancellationToken)
    {
        // 1. جلب العنصر (نستخدم المستودع الخاص بالـ Items)
        var results = await _unitOfWork.Items.FindWithDeletedAsync(i => i.Id == request.Id);

        if (results == null) return false;

        var entity = results.FirstOrDefault();
        if (entity == null) return false;

        // 2. Restore
        entity.IsDeleted = false;
        entity.DeletedAt = null;

        // 3. Update and save
        _unitOfWork.Items.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
}