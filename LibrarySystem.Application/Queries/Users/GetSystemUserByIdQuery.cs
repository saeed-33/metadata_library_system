using AutoMapper;
using LibrarySystem.Application.DTOs.Users;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Users;

public record GetSystemUserByIdQuery(int Id) : IRequest<UserResponse?>;

public class GetSystemUserByIdQueryHandler : IRequestHandler<GetSystemUserByIdQuery, UserResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetSystemUserByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<UserResponse?> Handle(
        GetSystemUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.SystemUsers.GetByIdAsync(request.Id);
        if (user == null) return null;

        var roles = await _identityService
            .GetRolesByExternalIdAsync(user.ExternalId);

        return _mapper.Map<UserResponse>(user) with { Roles = roles };
    }
}
