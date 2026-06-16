using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Media;

public record GetAllMediaWithDeletedQuery() : IRequest<IEnumerable<MediaAdminResponse>>;

public class GetAllMediaWithDeletedQueryHandler : IRequestHandler<GetAllMediaWithDeletedQuery, IEnumerable<MediaAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllMediaWithDeletedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MediaAdminResponse>> Handle(
        GetAllMediaWithDeletedQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch all media records including deleted
        var results = await _unitOfWork.Medias.FindWithDeletedAsync(x => true);
        var mediaList = results.ToList();

        if (!mediaList.Any())
            return new List<MediaAdminResponse>();

        // 2. Fetch ALL values including deleted
        var mediaIds = mediaList.Select(m => m.Id).ToList();
        var allValues = await _unitOfWork.Values.FindWithDeletedAsync(
            v => mediaIds.Contains(v.ResourceId),
            v => v.Property!
        );

        // 3. Group values by ResourceId
        var valuesByMediaId = allValues
            .GroupBy(v => v.ResourceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 4. Map media to Admin response manually
        var response = mediaList.Select(media => new MediaAdminResponse
        {
            Id = media.Id,
            ItemId = media.ItemId,
            StoragePath = media.StoragePath,
            FileName = media.FileName,
            IsDeleted = media.IsDeleted, // تمرير حالة الحذف للـ Media

            MetadataValues = valuesByMediaId.TryGetValue(media.Id, out var values)
                ? values.Select(v => new ItemAdminValueResponse
                {
                    PropertyId = v.PropertyId,
                    PropertyLabel = v.Property?.Label ?? string.Empty,
                    ValueText = v.ValueText,
                    Language = v.Language,
                    IsDeleted = v.IsDeleted // تمرير حالة الحذف للـ Metadata
                }).ToList()
                : new List<ItemAdminValueResponse>()
        }).ToList();

        return response;
    }
}