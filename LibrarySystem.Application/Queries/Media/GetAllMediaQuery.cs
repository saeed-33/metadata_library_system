using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Media;

public record GetAllMediaQuery() : IRequest<IEnumerable<MediaResponse>>;

public class GetAllMediaQueryHandler : IRequestHandler<GetAllMediaQuery, IEnumerable<MediaResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllMediaQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MediaResponse>> Handle(
        GetAllMediaQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch all media records
        var results = await _unitOfWork.Medias.FindAsync(x => true);
        var mediaList = results.ToList();

        if (!mediaList.Any())
            return new List<MediaResponse>();

        // 2. Fetch ALL values for ALL media records in one single DB call
        var mediaIds = mediaList.Select(m => m.Id).ToList();
        var allValues = await _unitOfWork.Values.FindAsync(
            v => mediaIds.Contains(v.ResourceId),
            v => v.Property!
        );

        // 3. Group values by ResourceId (MediaId) for fast lookup
        var valuesByMediaId = allValues
            .GroupBy(v => v.ResourceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 4. Map media to response manually and inject values
        var response = mediaList.Select(media => new MediaResponse
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

        return response;
    }
}