using AutoMapper;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Users;

public record UpdateSystemUserCommand(
    int Id,
    string FullName,
    string? Bio,
    string? ProfilePicturePath
) : IRequest<bool>;

public class UpdateSystemUserCommandHandler : IRequestHandler<UpdateSystemUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSystemUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateSystemUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.SystemUsers.GetByIdAsync(request.Id);
        if (user == null) return false;

        _mapper.Map(request, user);
        _unitOfWork.SystemUsers.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}