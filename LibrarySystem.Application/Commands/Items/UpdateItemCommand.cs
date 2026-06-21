using AutoMapper;
using LibrarySystem.Application.DTOs.Values;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands.Items;

public record UpdateItemCommand(
    int Id,
    int? TemplateId,
    List<CreateValueRequest> Values
) : IRequest<bool>;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch item
        var item = (await _unitOfWork.Items.FindAsync(
            i => i.Id == request.Id)).FirstOrDefault();
        if (item == null) return false;

        // 2. Validate all ValueResourceIds before making any changes
        foreach (var vReq in request.Values)
        {
            if (vReq.Type == "resource" && vReq.ValueResourceId.HasValue)
            {
                // Check across all resource types
                var existsAsItem = await _unitOfWork.Items.FindAsync(
                    r => r.Id == vReq.ValueResourceId.Value);
                var existsAsMedia = await _unitOfWork.Medias.FindAsync(
                    r => r.Id == vReq.ValueResourceId.Value);
                var existsAsItemSet = await _unitOfWork.ItemSets.GetByIdAsync(
                    vReq.ValueResourceId.Value);

                bool resourceExists = existsAsItem.Any()
                                   || existsAsMedia.Any()
                                   || existsAsItemSet != null;

                if (!resourceExists)
                    throw new InvalidOperationException(
                        $"Resource with id {vReq.ValueResourceId.Value} does not exist. " +
                        $"Cannot link property {vReq.PropertyId} to a non-existent resource.");
            }
        }

        // 3. Update item basic fields
        _mapper.Map(request, item);

        // 4. Delete old values
        var oldValues = await _unitOfWork.Values
            .FindAsync(v => v.ResourceId == item.Id);
        foreach (var oldVal in oldValues)
            _unitOfWork.Values.Delete(oldVal);

        // 5. Add new values
        foreach (var vReq in request.Values)
        {
            var newValue = _mapper.Map<Value>(vReq);
            newValue.ResourceId = item.Id;
            await _unitOfWork.Values.AddAsync(newValue);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
