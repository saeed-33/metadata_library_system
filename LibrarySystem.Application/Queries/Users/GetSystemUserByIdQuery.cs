using AutoMapper;
using LibrarySystem.Application.DTOs.Users;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Users;

public record GetSystemUserByIdQuery(int Id) : IRequest<UserResponse?>;

public class GetSystemUserByIdQueryHandler : IRequestHandler<GetSystemUserByIdQuery, UserResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSystemUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserResponse?> Handle(GetSystemUserByIdQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.SystemUsers.FindAsync(
            u => u.Id == request.Id,
            u => u.Roles // جلب قائمة الأدوار
        );

        var user = users.FirstOrDefault();
        return user == null ? null : _mapper.Map<UserResponse>(user);
    }
}