using MediatR;

public record DeleteItemCopyCommand(
    int Id
    ) : IRequest<bool>;
