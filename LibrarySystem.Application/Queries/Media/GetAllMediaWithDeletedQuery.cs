using AutoMapper;
using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Queries.Media;

public record GetAllMediaWithDeletedQuery() : IRequest<IEnumerable<MediaAdminResponse>>;

public class GetAllMediaWithDeletedQueryHandler : IRequestHandler<GetAllMediaWithDeletedQuery, IEnumerable<MediaAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMediaWithDeletedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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
            v => v.Property!);

        // 3. Group values by ResourceId
        var valuesByMediaId = allValues
            .GroupBy(v => v.ResourceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 4. Map media to Admin response and inject values
        var response = _mapper.Map<List<MediaAdminResponse>>(mediaList);
        for (int i = 0; i < mediaList.Count; i++)
        {
            var values = valuesByMediaId.TryGetValue(mediaList[i].Id, out var v)
                ? v
                : new List<Value>();
            response[i].MetadataValues = _mapper.Map<List<ItemAdminValueResponse>>(values);
        }

        return response;
    }
}
