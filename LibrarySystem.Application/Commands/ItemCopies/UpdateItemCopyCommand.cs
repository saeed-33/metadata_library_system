using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.ItemCopies;

public record UpdateItemCopyCommand(int Id, string Barcode, ItemCopyStatus Status, string? Notes) : IRequest<bool>;

public class UpdateItemCopyHandler : IRequestHandler<UpdateItemCopyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateItemCopyHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateItemCopyCommand request, CancellationToken ct)
    {
        var copy = await _unitOfWork.ItemCopies.GetByIdAsync(request.Id);
        if (copy == null) return false;

        _mapper.Map(request, copy);

        _unitOfWork.ItemCopies.Update(copy);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
