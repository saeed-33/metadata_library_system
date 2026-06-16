using LibrarySystem.Application.DTOs.Users;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Users;

public record GetAllWithDeletedUsersQuery() : IRequest<IEnumerable<UserAdminResponse>>;

public class GetAllWithDeletedUsersQueryHandler : IRequestHandler<GetAllWithDeletedUsersQuery, IEnumerable<UserAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public GetAllWithDeletedUsersQueryHandler(
        IUnitOfWork unitOfWork,
        IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<IEnumerable<UserAdminResponse>> Handle(
        GetAllWithDeletedUsersQuery request, // تم تصحيح اسم الريكويست هنا
        CancellationToken cancellationToken)
    {
        // تم تغيير الدالة لتجلب المستخدمين المحذوفين أيضاً
        var users = await _unitOfWork.SystemUsers.GetAllWithDeletedAsync();
        var result = new List<UserAdminResponse>();

        foreach (var user in users)
        {
            var roles = await _identityService
                .GetRolesByExternalIdAsync(user.ExternalId);

            result.Add(new UserAdminResponse(
                user.Id,
                user.ExternalId,
                user.FullName,
                user.Bio,
                user.ProfilePicturePath,
                user.IsDeleted, // تم تمرير خاصية IsDeleted هنا كمتغير سادس
                roles           // المتغير السابع
            ));
        }

        return result;
    }
}