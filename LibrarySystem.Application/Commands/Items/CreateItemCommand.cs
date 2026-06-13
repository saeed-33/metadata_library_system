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
    string? ValueText,      // للنصوص العادية (literal)
    string? ValueUri,       // للروابط الخارجية (uri)
    int? ValueResourceId,   // للربط مع مورد داخلي (resource)
    string Type = "literal", // "literal", "uri", or "resource"
    string Language = "ar"
);

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<int> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var item = new Item
        {
            TemplateId = request.TemplateId,
            OwnerId = request.OwnerId
        };

        await _unitOfWork.Items.AddAsync(item);

        // Save item first so it gets a real Id from the database
        await _unitOfWork.SaveChangesAsync();

        // Now item.Id is populated — safe to use as FK for Values
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
                ResourceId = item.Id  // ← use Id directly, not navigation property
            };
            await _unitOfWork.Values.AddAsync(value);
        }

        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}