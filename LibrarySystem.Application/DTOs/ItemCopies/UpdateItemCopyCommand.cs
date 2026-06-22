using MediatR;

public record UpdateItemCopyCommand(
    int Id,
    string Barcode,
    int Status,
    string? Notes
    ) : IRequest<bool>;
