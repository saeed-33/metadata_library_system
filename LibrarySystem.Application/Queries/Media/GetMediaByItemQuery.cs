using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Media;

public record GetMediaByItemQuery(int ItemId) : IRequest<IEnumerable<MediaResponse>>;

public class GetMediaByItemQueryHandler : IRequestHandler<GetMediaByItemQuery, IEnumerable<MediaResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMediaByItemQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MediaResponse>> Handle(
        GetMediaByItemQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch all media for this item — no Values include
        var mediaList = (await _unitOfWork.Medias.FindAsync(
            m => m.ItemId == request.ItemId)).ToList();

        if (!mediaList.Any()) return new List<MediaResponse>();

        // 2. Fetch all values for all media in one DB call
        var mediaIds = mediaList.Select(m => m.Id).ToList();
        var allValues = await _unitOfWork.Values.FindAsync(
            v => mediaIds.Contains(v.ResourceId),
            v => v.Property!
        );

        // 3. Group values by ResourceId
        var valuesByMediaId = allValues
            .GroupBy(v => v.ResourceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 4. Build response manually
        return mediaList.Select(media => new MediaResponse
        {
            Id = media.Id,
            ItemId = media.ItemId,
            StoragePath = media.StoragePath,
            FileName = media.FileName,
            MetadataValues = valuesByMediaId.TryGetValue(media.Id, out var values)
                ? values.Select(v => new ItemValueResponse
                {
                    PropertyId = v.PropertyId,
                    PropertyLabel = v.Property?.Label ?? string.Empty,
                    ValueText = v.ValueText,
                    Language = v.Language
                }).ToList()
                : new List<ItemValueResponse>()
        }).ToList();
    }
}