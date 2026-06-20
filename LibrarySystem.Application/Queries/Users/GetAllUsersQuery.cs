using AutoMapper;
using LibrarySystem.Application.DTOs.Users;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Users;

public record GetAllUsersQuery() : IRequest<IEnumerable<UserResponse>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResponse>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.SystemUsers.GetAllAsync();
        var result = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _identityService
                .GetRolesByExternalIdAsync(user.ExternalId);

            result.Add(_mapper.Map<UserResponse>(user) with { Roles = roles });
        }

        return result;
    }
}
