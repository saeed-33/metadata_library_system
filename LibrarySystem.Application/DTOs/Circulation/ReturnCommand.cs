using MediatR;

public record ReturnCommand(
    string Barcode
    ) : IRequest<bool>;
