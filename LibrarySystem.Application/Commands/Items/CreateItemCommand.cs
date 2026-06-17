using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands.Items;

public record CreateItemCommand(
    int? TemplateId,
    int? OwnerId,
    List<CreateValueRequest> Values
) : IRequest<int>;

public record CreateValueRequest(
    int PropertyId,
    string? ValueText,
    string? ValueUri,
    int? ValueResourceId,
    string Type = "literal",
    string Language = "ar"
);

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<int> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate all ValueResourceIds before saving anything
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

        // 2. Create and save item first to get real Id
        var item = new Item
        {
            TemplateId = request.TemplateId,
            OwnerId = request.OwnerId
        };

        await _unitOfWork.Items.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();

        // 3. Add values with validated resource links
        foreach (var vReq in request.Values)
        {
            var value = new Value
            {
                PropertyId = vReq.PropertyId,
                ValueText = vReq.ValueText,
                ValueUri = vReq.ValueUri,
                ValueResourceId = vReq.ValueResourceId,
                Type = vReq.Type,
                Language = vReq.Language,
                ResourceId = item.Id
            };
            await _unitOfWork.Values.AddAsync(value);
        }

        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}