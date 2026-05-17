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

        foreach (var vReq in request.Values)
        {
            var value = new Value
            {
                PropertyId = vReq.PropertyId,
                ValueText = vReq.ValueText,
                ValueUri = vReq.ValueUri,
                ValueResourceId = vReq.ValueResourceId,
                Type = vReq.Type, // هنا نحدد النوع (نص، رابط، أو مورد)
                Language = vReq.Language,
                Resource = item
            };
            await _unitOfWork.Values.AddAsync(value);
        }

        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}