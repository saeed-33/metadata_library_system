using MediatR;

public record UndeleteItemCopyCommand(
    int Id
    ) : IRequest<bool>;
