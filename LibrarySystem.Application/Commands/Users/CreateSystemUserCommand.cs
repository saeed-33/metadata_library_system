using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;
using AutoMapper;

namespace LibrarySystem.Application.Commands.Users;

public record CreateSystemUserCommand(
    string ExternalId,
    string FullName,
    string? Bio,
    string? ProfilePicturePath
) : IRequest<int>;

public class CreateSystemUserCommandHandler : IRequestHandler<CreateSystemUserCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSystemUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateSystemUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<SystemUser>(request);

        await _unitOfWork.SystemUsers.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user.Id;
    }
}