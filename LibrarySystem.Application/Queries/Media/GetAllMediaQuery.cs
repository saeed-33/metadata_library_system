using AutoMapper;
using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Queries.Media;

public record GetAllMediaQuery() : IRequest<IEnumerable<MediaResponse>>;

public class GetAllMediaQueryHandler : IRequestHandler<GetAllMediaQuery, IEnumerable<MediaResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMediaQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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
            v => v.Property!);

        // 3. Group values by ResourceId (MediaId) for fast lookup
        var valuesByMediaId = allValues
            .GroupBy(v => v.ResourceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 4. Map media to response and inject values
        var response = _mapper.Map<List<MediaResponse>>(mediaList);
        for (int i = 0; i < mediaList.Count; i++)
        {
            var values = valuesByMediaId.TryGetValue(mediaList[i].Id, out var v)
                ? v
                : new List<Value>();
            response[i].MetadataValues = _mapper.Map<List<ItemValueResponse>>(values);
        }

        return response;
    }
}
