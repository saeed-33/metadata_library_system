using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Media;

public record GetMediaByIdQuery(int Id) : IRequest<MediaResponse?>;

public class GetMediaByIdQueryHandler : IRequestHandler<GetMediaByIdQuery, MediaResponse?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMediaByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MediaResponse?> Handle(
        GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch media — no Values include
        var results = await _unitOfWork.Medias.FindAsync(m => m.Id == request.Id);
        var media = results.FirstOrDefault();
        if (media == null) return null;

        // 2. Fetch values separately
        var values = await _unitOfWork.Values.FindAsync(
            v => v.ResourceId == media.Id,
            v => v.Property!
        );

        // 3. Build response manually
        return new MediaResponse
        {
            Id = media.Id,
            ItemId = media.ItemId,
            StoragePath = media.StoragePath,
            FileName = media.FileName,
            MetadataValues = values.Select(v => new ItemValueResponse
            {
                PropertyId = v.PropertyId,
                PropertyLabel = v.Property?.Label ?? string.Empty,
                ValueText = v.ValueText,
                Language = v.Language
            }).ToList()
        };
    }
}