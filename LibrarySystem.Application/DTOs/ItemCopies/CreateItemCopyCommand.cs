using MediatR;

public record CreateItemCopyCommand(
    int ItemId,
    string Barcode,
    string? Notes
    ) : IRequest<int>;
