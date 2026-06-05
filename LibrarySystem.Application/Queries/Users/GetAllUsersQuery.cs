using AutoMapper;
using LibrarySystem.Application.DTOs.Users;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Users;

public record GetAllUsersQuery() : IRequest<IEnumerable<UserResponse>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        // Include Roles so UserResponse.Roles gets populated
        var users = await _unitOfWork.SystemUsers.FindAsync(
            u => true,
            u => u.Roles
        );

        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }
}